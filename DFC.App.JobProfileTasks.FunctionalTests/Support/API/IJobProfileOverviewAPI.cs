using DFC.App.JobProfileOverview.Tests.IntegrationTests.API.Model.API;
using RestSharp;
using System.Threading.Tasks;

namespace DFC.App.JobProfileOverview.Tests.IntegrationTests.API.Support.API
{
    public interface IJobProfileOverviewAPI
    {
        Task<IRestResponse<JobProfileOverviewApiResponse>> GetById(string id);
    }
}
