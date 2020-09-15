using DFC.App.JobProfileTasks.FunctionalTests.Model.API;
using RestSharp;
using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.FunctionalTests.Support.API
{
    public interface IJobProfileOverviewAPI
    {
        Task<IRestResponse<JobProfileOverviewApiResponse>> GetById(string id);
    }
}
