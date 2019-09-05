using DFC.App.JobProfileTasks.Data.Contracts;
using DFC.App.JobProfileTasks.Data.Models;
using System;
using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.DraftSegmentService
{
    public class DraftJobProfileTasksSegmentService : IDraftJobProfileTasksSegmentService
    {
        public Task<JobProfileTasksSegmentModel> GetSitefinityData(string canonicalName)
        {
            throw new NotImplementedException();
        }
    }
}
