using Microsoft.Azure.ServiceBus;

namespace DFC.App.JobProfileOverview.Tests.IntegrationTests.API.Support.AzureServiceBus.ServiceBusFactory.Interface
{
    public interface ITopicClientFactory
    {
        ITopicClient Create(string connectionString);
    }
}
