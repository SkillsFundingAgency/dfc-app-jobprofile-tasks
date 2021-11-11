using AutoMapper;
using DFC.App.JobProfileTasks.Data.Models;
using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using DFC.App.JobProfileTasks.Data.Models.ServiceBusModels;
using DFC.App.JobProfileTasks.Repository.CosmosDb;
using DFC.App.JobProfileTasks.SegmentService;
using DFC.Logger.AppInsights.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfileTasks
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public const string CosmosDbConfigAppSettings = "Configuration:CosmosDbConnections:JobProfileSegment";
        public const string ServiceBusOptionsAppSettings = "ServiceBusOptions";

        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMapper mapper)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();

                // add the default route
                endpoints.MapControllerRoute("default", "{controller=Health}/{action=Ping}");
            });

            mapper?.ConfigurationProvider.AssertConfigurationIsValid();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var serviceBusOptions = configuration.GetSection(ServiceBusOptionsAppSettings).Get<ServiceBusOptions>();
            var cosmosDbConnection = configuration.GetSection(CosmosDbConfigAppSettings).Get<CosmosDbConnection>();
            var retryOptions = new RetryOptions { MaxRetryAttemptsOnThrottledRequests = 20, MaxRetryWaitTimeInSeconds = 60 };
            var documentClient = new DocumentClient(new Uri(cosmosDbConnection.EndpointUrl), cosmosDbConnection.AccessKey, new ConnectionPolicy { RetryOptions = retryOptions }); var topicClient = new TopicClient(serviceBusOptions.ServiceBusConnectionString, serviceBusOptions.TopicName);

            services.AddSingleton(cosmosDbConnection);
            services.AddApplicationInsightsTelemetry();
            services.AddSingleton<IDocumentClient>(documentClient);
            services.AddSingleton<ITopicClient>(topicClient);
            services.AddSingleton<ICosmosRepository<JobProfileTasksSegmentModel>, CosmosRepository<JobProfileTasksSegmentModel>>();
            services.AddScoped<IJobProfileTasksSegmentService, JobProfileTasksSegmentService>();
            services.AddAutoMapper(typeof(Startup).Assembly);
            services.AddScoped<IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel>, JobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel>>();
            services.AddDFCLogging(configuration["ApplicationInsights:InstrumentationKey"]);

            services.AddMvc().AddNewtonsoftJson().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }
    }
}