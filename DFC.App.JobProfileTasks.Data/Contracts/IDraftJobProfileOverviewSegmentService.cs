using DFC.App.JobProfileTasks.Data.Models;
using System.Threading.Tasks;

namespace DFC.App.JobProfileOverview.Data.Contracts
{
    public interface IDraftJobProfileOverviewSegmentService
    {
        Task<JobProfileTaskSegmentModel> GetSitefinityData(string canonicalName);
    }
}
