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

        Task<UpsertJobProfileTasksModelResponse> UpsertAsync(JobProfileTasksSegmentModel tasksSegmentModel);

        Task<bool> DeleteAsync(Guid documentId);

        Task<HttpStatusCode> UpdateUniform(Guid documentId, JobProfileTasksDataUniformSegmentModel model);

        Task<HttpStatusCode> UpdateLocation(Guid documentId, JobProfileTasksDataLocationSegmentModel model);

        Task<HttpStatusCode> UpdateEnvironment(Guid documentId, JobProfileTasksDataEnvironmentSegmentModel model);

        Task<HttpStatusCode> DeleteUniform(Guid jobProfileId, Guid uniformId);

        Task<HttpStatusCode> DeleteLocation(Guid jobProfileId, Guid locationId);

        Task<HttpStatusCode> DeleteEnvironment(Guid jobProfileId, Guid environmentId);
    }
}