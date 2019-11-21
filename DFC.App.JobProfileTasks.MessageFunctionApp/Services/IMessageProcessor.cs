using DFC.App.JobProfileTasks.Data.Enums;
using DFC.App.JobProfileTasks.Data.Models.PatchModels;
using DFC.App.JobProfileTasks.Data.Models.ServiceBusModels;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.MessageFunctionApp.Services
{
    public interface IMessageProcessor
    {
        Task<HttpStatusCode> Delete(Guid jobProfileId);

        Task<HttpStatusCode> Save(JobProfileServiceBusModel message, MessageContentType messageContentType, long sequenceNumber);

        Task<HttpStatusCode> DeleteUniform(Guid jobProfileId, Guid uniformId);

        Task<HttpStatusCode> DeleteLocation(Guid jobProfileId, Guid locationId);

        Task<HttpStatusCode> DeleteEnvironment(Guid jobProfileId, Guid environmentId);

        Task<HttpStatusCode> PatchUniform(PatchUniformModel message);

        Task<HttpStatusCode> PatchLocation(PatchLocationModel message);

        Task<HttpStatusCode> PatchEnvironment(PatchEnvironmentsModel message);
    }
}
