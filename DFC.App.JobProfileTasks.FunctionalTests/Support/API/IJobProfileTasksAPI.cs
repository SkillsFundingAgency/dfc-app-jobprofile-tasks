using DFC.App.JobProfileTasks.FunctionalTests.Model.API;
using RestSharp;
using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.FunctionalTests.Support.API
{
    public interface IJobProfileTasksAPI
    {
        Task<IRestResponse<JobProfileTasksResponse>> GetById(string id);
    }
}
