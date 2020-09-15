using Microsoft.Azure.ServiceBus;

namespace DFC.App.JobProfileTasks.FunctionalTests.Support.AzureServiceBus.ServiceBusFactory.Interface
{
    public interface ITopicClientFactory
    {
        ITopicClient Create(string connectionString);
    }
}
