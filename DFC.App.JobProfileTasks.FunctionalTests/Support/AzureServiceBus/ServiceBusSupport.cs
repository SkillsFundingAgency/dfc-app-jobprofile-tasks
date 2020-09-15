using DFC.App.JobProfileOverview.Tests.IntegrationTests.API.Model.Support;
using DFC.App.JobProfileOverview.Tests.IntegrationTests.API.Support.AzureServiceBus.ServiceBusFactory.Interface;
using Microsoft.Azure.ServiceBus;
using System.Threading.Tasks;

namespace DFC.App.JobProfileOverview.Tests.IntegrationTests.API.Support.AzureServiceBus
{
    public class ServiceBusSupport : IServiceBusSupport
    {
        private ITopicClientFactory topicClientFactory;
        private AppSettings appSettings;

        public ServiceBusSupport(ITopicClientFactory topicClientFactory, AppSettings appSettings)
        {
            this.topicClientFactory = topicClientFactory;
            this.appSettings = appSettings;
        }

        public async Task SendMessage(Message message)
        {
            ITopicClient topicClient = this.topicClientFactory.Create(this.appSettings.ServiceBusConfig.ConnectionString);
            await topicClient.SendAsync(message).ConfigureAwait(false);
        }
    }
}
