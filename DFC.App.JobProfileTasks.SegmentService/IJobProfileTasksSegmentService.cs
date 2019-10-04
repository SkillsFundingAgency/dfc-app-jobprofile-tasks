using DFC.App.JobProfileTasks.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.SegmentService
{
    public interface IJobProfileTasksSegmentService
    {
        Task<bool> PingAsync();

        Task<IEnumerable<JobProfileTasksSegmentModel>> GetAllAsync();

        Task<JobProfileTasksSegmentModel> GetByIdAsync(Guid documentId);

        Task<JobProfileTasksSegmentModel> GetByNameAsync(string canonicalName, bool isDraft = false);

        Task<UpsertJobProfileTasksModelResponse> UpsertAsync(JobProfileTasksSegmentModel tasksSegmentModel);

        Task<bool> DeleteAsync(Guid documentId);
    }
}