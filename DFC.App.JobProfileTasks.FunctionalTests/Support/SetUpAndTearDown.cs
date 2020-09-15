using DFC.App.JobProfileTasks.FunctionalTests.Model.ContentType.JobProfile;
using DFC.App.JobProfileTasks.FunctionalTests.Model.Support;
using DFC.App.JobProfileTasks.FunctionalTests.Support.AzureServiceBus;
using DFC.App.JobProfileTasks.FunctionalTests.Support.AzureServiceBus.ServiceBusFactory;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using Environment = DFC.App.JobProfileTasks.FunctionalTests.Model.ContentType.JobProfile.Environment;

namespace DFC.App.JobProfileTasks.FunctionalTests.Support
{
    public class SetUpAndTearDown
    {
        protected CommonAction CommonAction { get; set; } = new CommonAction();

        protected AppSettings AppSettings { get; set; }

        protected JobProfileContentType WakeUpJobProfile { get; set; }

        protected JobProfileContentType JobProfile { get; set; }

        protected ServiceBusSupport ServiceBus { get; set; }

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            // Get settings from appsettings
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            this.AppSettings = configuration.Get<AppSettings>();

            this.ServiceBus = new ServiceBusSupport(new TopicClientFactory(), this.AppSettings);

            // Send wake up job profile
            this.WakeUpJobProfile = this.CommonAction.GetResource<JobProfileContentType>("JobProfileTemplate");
            this.WakeUpJobProfile.JobProfileId = Guid.NewGuid().ToString();
            this.WakeUpJobProfile.CanonicalName = this.CommonAction.RandomString(10).ToLowerInvariant();
            var jobProfileMessageBody = this.CommonAction.ConvertObjectToByteArray(this.WakeUpJobProfile);
            var message = new MessageFactory().Create(this.WakeUpJobProfile.JobProfileId, jobProfileMessageBody, "Published", "JobProfile");
            await this.ServiceBus.SendMessage(message).ConfigureAwait(false);
            await Task.Delay(TimeSpan.FromMinutes(this.AppSettings.DeploymentWaitInMinutes)).ConfigureAwait(true);

            // Generate a test job profile
            this.JobProfile = this.CommonAction.GetResource<JobProfileContentType>("JobProfileTemplate");
            this.JobProfile.JobProfileId = Guid.NewGuid().ToString();
            this.JobProfile.CanonicalName = this.CommonAction.RandomString(10).ToLowerInvariant();

            var uniform = new Uniform()
            {
                Description = "This is a test uniform description",
                Id = Guid.NewGuid().ToString(),
                IsNegative = false,
                Title = "This is a test uniform title",
                Url = new Uri($"https://{this.CommonAction.RandomString(10)}.com/"),
            };
            
            var location = new Location()
            {
                Description = "This is a test location description",
                Id = Guid.NewGuid().ToString(),
                IsNegative = false,
                Title = "This is a test location title",
                Url = new Uri($"https://{this.CommonAction.RandomString(10)}.com/"),
            };
            
            var environment = new Environment()
            {
                Description = "This is a test environment description",
                Id = Guid.NewGuid().ToString(),
                IsNegative = false,
                Title = "This is a test environment title",
                Url = new Uri($"https://{this.CommonAction.RandomString(10)}.com/"),
            };

            this.JobProfile.WhatYouWillDoData.Uniforms.Add(uniform);
            this.JobProfile.WhatYouWillDoData.Locations.Add(location);
            this.JobProfile.WhatYouWillDoData.Environments.Add(environment);

            // Send job profile to the service bus
            jobProfileMessageBody = this.CommonAction.ConvertObjectToByteArray(this.JobProfile);
            message = new MessageFactory().Create(this.JobProfile.JobProfileId, jobProfileMessageBody, "Published", "JobProfile");
            await this.ServiceBus.SendMessage(message).ConfigureAwait(false);
            await Task.Delay(10000).ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            // Delete wake up job profile
            var wakeUpJobProfileDelete = this.CommonAction.GetResource<JobProfileContentType>("JobProfileTemplate");
            wakeUpJobProfileDelete.JobProfileId = this.WakeUpJobProfile.JobProfileId;
            var messageBody = this.CommonAction.ConvertObjectToByteArray(wakeUpJobProfileDelete);
            var message = new MessageFactory().Create(this.WakeUpJobProfile.JobProfileId, messageBody, "Deleted", "JobProfile");
            await this.ServiceBus.SendMessage(message).ConfigureAwait(false);

            // Delete test job profile
            var jobProfileDelete = this.CommonAction.GetResource<JobProfileContentType>("JobProfileTemplate");
            jobProfileDelete.JobProfileId = this.JobProfile.JobProfileId;
            messageBody = this.CommonAction.ConvertObjectToByteArray(jobProfileDelete);
            message = new MessageFactory().Create(this.JobProfile.JobProfileId, messageBody, "Deleted", "JobProfile");
            await this.ServiceBus.SendMessage(message).ConfigureAwait(false);
        }
    }
}
