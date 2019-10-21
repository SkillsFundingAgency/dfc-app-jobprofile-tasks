using DFC.App.JobProfileTasks.Data.Models;
using DFC.App.JobProfileTasks.Data.Models.PatchModels;
using System;
using System.Collections.Generic;
using System.Net;
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

        Task<HttpStatusCode> Update(PatchUniformModel patchUniform);

        Task<HttpStatusCode> Delete(Guid jobProfileId, Guid uniformId);
    }
}