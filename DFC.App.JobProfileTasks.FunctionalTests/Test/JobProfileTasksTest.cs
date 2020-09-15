using DFC.App.JobProfileTasks.FunctionalTests.Model.MessageBody;
using DFC.App.JobProfileTasks.FunctionalTests.Support;
using DFC.App.JobProfileTasks.FunctionalTests.Support.API;
using DFC.App.JobProfileTasks.FunctionalTests.Support.API.RestFactory;
using DFC.App.JobProfileTasks.FunctionalTests.Support.AzureServiceBus.ServiceBusFactory;
using NUnit.Framework;
using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.FunctionalTests.Test
{
    public class JobProfileTasksTest : SetUpAndTearDown
    {
        private IJobProfileTasksAPI jobProfileTasksApi;

        [SetUp]
        public void SetUp()
        {
            this.jobProfileTasksApi = new JobProfileTasksAPI(new RestClientFactory(), new RestRequestFactory(), this.AppSettings);
        }

        [Test]
        public async Task UniformCType()
        {
            var uniformMessageBody = new UniformMessageBody()
            {
                JobProfileId = this.JobProfile.JobProfileId,
                JobProfileTitle = this.JobProfile.Title,
                Description = "This is updated uniform data",
                IsNegative = false,
                Id = this.JobProfile.WhatYouWillDoData.Uniforms[0].Id,
                Title = this.JobProfile.WhatYouWillDoData.Uniforms[0].Title,
                Url = this.JobProfile.WhatYouWillDoData.Uniforms[0].Url.ToString(),
            };

            var messageBody = this.CommonAction.ConvertObjectToByteArray(uniformMessageBody);
            var message = new MessageFactory().Create(this.JobProfile.JobProfileId, messageBody, "Published", "Uniform");
            await this.ServiceBus.SendMessage(message).ConfigureAwait(false);
            await Task.Delay(5000).ConfigureAwait(true);
            var response = await this.jobProfileTasksApi.GetById(this.JobProfile.JobProfileId).ConfigureAwait(true);

            Assert.AreEqual($"You may need to wear {uniformMessageBody.Description}.", response.Data.workingEnvironment.uniform);
        }
        
        [Test]
        public async Task LocationCType()
        {
            var locationMessageBody = new LocationMessageBody()
            {
                JobProfileId = this.JobProfile.JobProfileId,
                JobProfileTitle = this.JobProfile.Title,
                Description = "This is updated location data",
                IsNegative = false,
                Id = this.JobProfile.WhatYouWillDoData.Locations[0].Id,
                Title = this.JobProfile.WhatYouWillDoData.Locations[0].Title,
                Url = this.JobProfile.WhatYouWillDoData.Locations[0].Url.ToString(),
            };

            var messageBody = this.CommonAction.ConvertObjectToByteArray(locationMessageBody);
            var message = new MessageFactory().Create(this.JobProfile.JobProfileId, messageBody, "Published", "Location");
            await this.ServiceBus.SendMessage(message).ConfigureAwait(false);
            await Task.Delay(5000).ConfigureAwait(true);
            var response = await this.jobProfileTasksApi.GetById(this.JobProfile.JobProfileId).ConfigureAwait(true);

            Assert.AreEqual($"You could work {locationMessageBody.Description}.", response.Data.workingEnvironment.location);
        }
        
        [Test]
        public async Task EnvironmentCType()
        {
            var environmentMessageBody = new EnvironmentMessageBody()
            {
                JobProfileId = this.JobProfile.JobProfileId,
                JobProfileTitle = this.JobProfile.Title,
                Description = "This is updated environment data",
                IsNegative = false,
                Id = this.JobProfile.WhatYouWillDoData.Environments[0].Id,
                Title = this.JobProfile.WhatYouWillDoData.Environments[0].Title,
                Url = this.JobProfile.WhatYouWillDoData.Environments[0].Url.ToString(),
            };

            var messageBody = this.CommonAction.ConvertObjectToByteArray(environmentMessageBody);
            var message = new MessageFactory().Create(this.JobProfile.JobProfileId, messageBody, "Published", "Environment");
            await this.ServiceBus.SendMessage(message).ConfigureAwait(false);
            await Task.Delay(5000).ConfigureAwait(true);
            var response = await this.jobProfileTasksApi.GetById(this.JobProfile.JobProfileId).ConfigureAwait(true);

            Assert.AreEqual($"Your working environment may be {environmentMessageBody.Description}.", response.Data.workingEnvironment.environment);
        }
    }
}