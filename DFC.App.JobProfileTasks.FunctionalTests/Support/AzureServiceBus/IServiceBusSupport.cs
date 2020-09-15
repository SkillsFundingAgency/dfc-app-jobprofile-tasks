using Microsoft.Azure.ServiceBus;
using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.FunctionalTests.Support.AzureServiceBus
{
    public interface IServiceBusSupport
    {
        Task SendMessage(Message message);
    }
}
