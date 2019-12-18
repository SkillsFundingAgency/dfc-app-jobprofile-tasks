using Microsoft.Azure.ServiceBus;

namespace DFC.App.JobProfileTasks.MessageFunctionApp.Services
{
    public interface IMessagePropertiesService
    {
        long GetSequenceNumber(Message message);
    }
}