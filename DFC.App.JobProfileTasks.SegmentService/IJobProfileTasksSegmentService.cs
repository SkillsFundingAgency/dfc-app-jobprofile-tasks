using DFC.App.JobProfileTasks.Data.Models.PatchModels;
using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
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

        Task<JobProfileTasksSegmentModel> GetByNameAsync(string canonicalName);

        Task<HttpStatusCode> UpsertAsync(JobProfileTasksSegmentModel tasksSegmentModel);

        Task<bool> DeleteAsync(Guid documentId);

        Task<HttpStatusCode> PatchUniformAsync(PatchUniformModel patchModel, Guid documentId);

        Task<HttpStatusCode> PatchLocationAsync(PatchLocationModel model, Guid documentId);

        Task<HttpStatusCode> PatchEnvironmentAsync(PatchEnvironmentsModel model, Guid documentId);
    }
}