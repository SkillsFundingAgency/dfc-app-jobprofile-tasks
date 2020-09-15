using DFC.App.JobProfileTasks.FunctionalTests.Model.Support;
using DFC.App.JobProfileTasks.FunctionalTests.Support.AzureServiceBus.ServiceBusFactory.Interface;
using Microsoft.Azure.ServiceBus;
using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.FunctionalTests.Support.AzureServiceBus
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
