using DFC.App.JobProfileOverview.Tests.IntegrationTests.API.Model.Classification;
using DFC.App.JobProfileOverview.Tests.IntegrationTests.API.Model.ContentType;
using DFC.App.JobProfileOverview.Tests.IntegrationTests.API.Support;
using DFC.App.JobProfileOverview.Tests.IntegrationTests.API.Support.API;
using DFC.App.JobProfileOverview.Tests.IntegrationTests.API.Support.API.RestFactory;
using DFC.App.JobProfileOverview.Tests.IntegrationTests.API.Support.AzureServiceBus.ServiceBusFactory;
using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfileOverview.Tests.IntegrationTests.API.Test
{
    public class JobProfileOverviewTest : SetUpAndTearDown
    {
        private IJobProfileOverviewAPI jobProfileOverviewApi;

        [SetUp]
        public void SetUp()
        {
            this.jobProfileOverviewApi = new JobProfileOverviewAPI(new RestClientFactory(), new RestRequestFactory(), this.AppSettings);
        }

        [Test]
        public async Task JobProfileOverviewJobProfileSOC()
        {
            var socCode = this.CommonAction.RandomString(5);
            var socCodeContentType = new SOCCodeContentType()
            {
                SOCCode = socCode,
                Id = this.JobProfile?.SocCodeData.Id,
                JobProfileId = this.JobProfile.JobProfileId,
                JobProfileTitle = this.JobProfile.Title,
                UrlName = this.JobProfile.SocCodeData.UrlName,
                Title = socCode,
                Description = "This is an updated SOC code",
                ONetOccupationalCode = "12.1234-00",
                ApprenticeshipFramework = this.JobProfile.SocCodeData.ApprenticeshipFramework,
                ApprenticeshipStandards = this.JobProfile.SocCodeData.ApprenticeshipStandards,
            };

            var messageBody = this.CommonAction.ConvertObjectToByteArray(socCodeContentType);
            var message = new MessageFactory().Create(this.JobProfile.JobProfileId, messageBody, "Published", "JobProfileSoc");
            await this.ServiceBus.SendMessage(message).ConfigureAwait(false);
            await Task.Delay(10000).ConfigureAwait(false);
            var apiResponse = await this.jobProfileOverviewApi.GetById(this.JobProfile.JobProfileId).ConfigureAwait(false);

            Assert.AreEqual(HttpStatusCode.OK, apiResponse.StatusCode);
            Assert.AreEqual(socCodeContentType.SOCCode.Substring(0, 4), apiResponse.Data.SOC);
            Assert.AreEqual(socCodeContentType.ONetOccupationalCode, apiResponse.Data.ONetOccupationalCode);
        }

        [Test]
        public async Task JobProfileOverviewWorkingHoursDetails()
        {
            var workingHoursDetailsClassification = new WorkingHoursDetailsClassification()
            {
                Id = this.JobProfile?.WorkingHoursDetails[0].Id,
                Description = "This is an updated working course detail",
                JobProfileId = this.JobProfile.JobProfileId,
                JobProfileTitle = this.JobProfile.Title,
                Title = "This is an updated working course detail title",
                Url = this.JobProfile.WorkingHoursDetails[0].Url,
            };

            var messageBody = this.CommonAction.ConvertObjectToByteArray(workingHoursDetailsClassification);
            var message = new MessageFactory().Create(this.JobProfile.JobProfileId, messageBody, "Published", "WorkingHoursDetails");
            await this.ServiceBus.SendMessage(message).ConfigureAwait(false);
            await Task.Delay(10000).ConfigureAwait(false);
            var apiResponse = await this.jobProfileOverviewApi.GetById(this.JobProfile.JobProfileId).ConfigureAwait(false);

            Assert.AreEqual(HttpStatusCode.OK, apiResponse.StatusCode);
            Assert.AreEqual(workingHoursDetailsClassification.Title, apiResponse.Data.WorkingHoursDetails);
        }

        [Test]
        public async Task JobProfileOverviewWorkingPattern()
        {
            var workingPatternClassification = new WorkingPatternClassification()
            {
                Id = this.JobProfile?.WorkingPattern[0].Id,
                Description = "This is an updated working pattern classification",
                JobProfileId = this.JobProfile.JobProfileId,
                JobProfileTitle = this.JobProfile.Title,
                Title = "This is an updated working pattern classification title",
                Url = this.JobProfile.WorkingPattern[0].Url,
            };

            var messageBody = this.CommonAction.ConvertObjectToByteArray(workingPatternClassification);
            var message = new MessageFactory().Create(this.JobProfile.JobProfileId, messageBody, "Published", "WorkingPattern");
            await this.ServiceBus.SendMessage(message).ConfigureAwait(false);
            await Task.Delay(10000).ConfigureAwait(false);
            var apiResponse = await this.jobProfileOverviewApi.GetById(this.JobProfile.JobProfileId).ConfigureAwait(false);

            Assert.AreEqual(HttpStatusCode.OK, apiResponse.StatusCode);
            Assert.AreEqual(workingPatternClassification.Title, apiResponse.Data.WorkingPattern);
        }

        [Test]
        public async Task JobProfileOverviewWorkingPatternDetails()
        {
            var workingPatternDetailClassification = new WorkingPatternDetailClassification()
            {
                Id = this.JobProfile?.WorkingPatternDetails[0].Id,
                Description = "This is an updated working pattern detail",
                JobProfileId = this.JobProfile.JobProfileId,
                JobProfileTitle = this.JobProfile.Title,
                Title = "This is an updated working pattern detail title",
                Url = this.JobProfile.WorkingPatternDetails[0].Url,
            };

            var messageBody = this.CommonAction.ConvertObjectToByteArray(workingPatternDetailClassification);
            var message = new MessageFactory().Create(this.JobProfile.JobProfileId, messageBody, "Published", "WorkingPatternDetails");
            await this.ServiceBus.SendMessage(message).ConfigureAwait(false);
            await Task.Delay(10000).ConfigureAwait(false);
            var apiResponse = await this.jobProfileOverviewApi.GetById(this.JobProfile.JobProfileId).ConfigureAwait(false);

            Assert.AreEqual(HttpStatusCode.OK, apiResponse.StatusCode);
            Assert.AreEqual(workingPatternDetailClassification.Title, apiResponse.Data.WorkingPatternDetails);
        }
    }
}