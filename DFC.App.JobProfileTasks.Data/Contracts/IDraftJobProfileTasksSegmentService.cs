using DFC.App.JobProfileTasks.Data.Models;
using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.Data.Contracts
{
    public interface IDraftJobProfileTasksSegmentService
    {
        Task<JobProfileTasksSegmentModel> GetSitefinityData(string canonicalName);
    }
}
