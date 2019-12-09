using AutoMapper;
using DFC.App.JobProfileTasks.Common.Contracts;
using DFC.App.JobProfileTasks.Common.Services;
using DFC.App.JobProfileTasks.MessageFunctionApp.HttpClientPolicies;
using DFC.App.JobProfileTasks.MessageFunctionApp.Services;
using DFC.Functions.DI.Standard;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

[assembly: WebJobsStartup(typeof(DFC.App.JobProfileTasks.MessageFunctionApp.Startup), "Web Jobs Extension Startup")]

namespace DFC.App.JobProfileTasks.MessageFunctionApp
{
    [ExcludeFromCodeCoverage]
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var jobProfileClientOptions = configuration.GetSection("JobProfileTasksSegmentClientOptions").Get<JobProfileClientOptions>();

            builder.AddDependencyInjection();

            builder?.Services.AddSingleton(jobProfileClientOptions);
            builder?.Services.AddAutoMapper(typeof(Startup).Assembly);
            builder?.Services.AddTransient<IMessagePreProcessor, MessagePreProcessor>();
            builder?.Services.AddTransient<IMessageProcessor, MessageProcessor>();
            builder?.Services.AddScoped<ILogService, LogService>();
            builder?.Services.AddScoped<ICorrelationIdProvider, InMemoryCorrelationIdProvider>();
            builder?.Services.AddTransient(provider => new HttpClient());
            builder?.Services.AddScoped<IHttpClientService, HttpClientService>();
            builder?.Services.AddSingleton<IMappingService, MappingService>();
            builder?.Services.AddSingleton<IMessagePropertiesService, MessagePropertiesService>();
        }
    }
}