using Microsoft.Azure.ServiceBus;
using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.MessageFunctionApp.Services
{
    public interface IMessagePreProcessor
    {
        Task Process(Message sitefinityMessage);
    }
}