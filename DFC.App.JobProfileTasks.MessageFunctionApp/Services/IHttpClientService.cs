using DFC.App.JobProfileTasks.Data.Models.PatchModels;
using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.MessageFunctionApp.Services
{
    public interface IHttpClientService
    {
        Task<HttpStatusCode> PostAsync(JobProfileTasksSegmentModel tasksSegmentModel);

        Task<HttpStatusCode> PutAsync(JobProfileTasksSegmentModel tasksSegmentModel);

        Task<HttpStatusCode> PatchAsync<T>(T patchModel, string patchTypeEndpoint)
            where T : BasePatchModel;

        Task<HttpStatusCode> DeleteAsync(Guid id);
    }
}