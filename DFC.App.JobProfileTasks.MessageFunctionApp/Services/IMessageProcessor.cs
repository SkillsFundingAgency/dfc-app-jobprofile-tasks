using DFC.App.JobProfileTasks.Data.Enums;
using DFC.App.JobProfileTasks.Data.ServiceBusModels;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.MessageFunctionApp.Services
{
    public interface IMessageProcessor
    {
        Task<HttpStatusCode> Delete(Guid jobProfileId);

        Task<HttpStatusCode> Save(
            JobProfileServiceBusModel message,
            MessageContentType messageContentType,
            Guid jobProfileId,
            long sequenceNumber);
    }
}
