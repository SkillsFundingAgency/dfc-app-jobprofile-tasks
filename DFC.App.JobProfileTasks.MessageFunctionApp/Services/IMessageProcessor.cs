using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.MessageFunctionApp.Services
{
    public interface IMessageProcessor
    {
        Task<HttpStatusCode> ProcessAsync(
            string message,
            string messageAction,
            string messageContentType,
            string jobProfileId,
            long sequenceNumber);
    }
}
