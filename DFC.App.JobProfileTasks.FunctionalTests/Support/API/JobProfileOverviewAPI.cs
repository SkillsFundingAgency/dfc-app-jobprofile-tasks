using DFC.App.JobProfileOverview.Tests.IntegrationTests.API.Model.API;
using DFC.App.JobProfileOverview.Tests.IntegrationTests.API.Model.Support;
using DFC.App.JobProfileOverview.Tests.IntegrationTests.API.Support.API.RestFactory.Interface;
using RestSharp;
using System.Threading.Tasks;

namespace DFC.App.JobProfileOverview.Tests.IntegrationTests.API.Support.API
{
    public class JobProfileOverviewAPI : IJobProfileOverviewAPI
    {
        private IRestClientFactory restClientFactory;
        private IRestRequestFactory restRequestFactory;
        private AppSettings appSettings;

        public JobProfileOverviewAPI(IRestClientFactory restClientFactory, IRestRequestFactory restRequestFactory, AppSettings appSettings)
        {
            this.restClientFactory = restClientFactory;
            this.restRequestFactory = restRequestFactory;
            this.appSettings = appSettings;
        }

        public async Task<IRestResponse<JobProfileOverviewApiResponse>> GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            var restClient = this.restClientFactory.Create(this.appSettings.APIConfig.EndpointBaseUrl);
            var restRequest = this.restRequestFactory.Create($"/segment/{id}/contents");
            restRequest.AddHeader("Accept", "application/json");
            return await Task.Run(() => restClient.Execute<JobProfileOverviewApiResponse>(restRequest)).ConfigureAwait(false);
        }
    }
}
