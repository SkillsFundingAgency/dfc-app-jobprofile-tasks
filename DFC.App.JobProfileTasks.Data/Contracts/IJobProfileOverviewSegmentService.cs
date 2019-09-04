using DFC.App.JobProfileTasks.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.Data.Contracts
{
    public interface IJobProfileOverviewSegmentService
    {
        Task<IEnumerable<JobProfileTaskSegmentModel>> GetAllAsync();

        Task<JobProfileTaskSegmentModel> GetByIdAsync(Guid documentId);

        Task<JobProfileTaskSegmentModel> GetByNameAsync(string canonicalName, bool isDraft = false);
    }
}
