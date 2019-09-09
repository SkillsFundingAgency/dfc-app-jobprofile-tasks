using DFC.App.JobProfileTasks.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.Data.Contracts
{
    public interface IJobProfileTasksSegmentService
    {
        Task<IEnumerable<JobProfileTasksSegmentModel>> GetAllAsync();

        Task<JobProfileTasksSegmentModel> GetByIdAsync(Guid documentId);

        Task<JobProfileTasksSegmentModel> GetByNameAsync(string canonicalName, bool isDraft = false);
    }
}
