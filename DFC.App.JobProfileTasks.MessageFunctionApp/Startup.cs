using AutoMapper;
using DFC.App.JobProfileTasks.MessageFunctionApp.HttpClientPolicies;
using DFC.App.JobProfileTasks.MessageFunctionApp.Services;
using DFC.Functions.DI.Standard;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: WebJobsStartup(typeof(DFC.App.JobProfileTasks.MessageFunctionApp.Startup), "Web Jobs Extension Startup")]

namespace DFC.App.JobProfileTasks.MessageFunctionApp
{
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

            builder?.Services.AddHttpClient(nameof(MessageProcessor), httpClient =>
            {
                httpClient.Timeout = jobProfileClientOptions.Timeout;
                httpClient.BaseAddress = jobProfileClientOptions.BaseAddress;
            });
        }
    }
}