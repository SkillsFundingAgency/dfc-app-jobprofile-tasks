using DFC.App.RelatedCareers.Tests.IntegrationTests.API.Model.Support;
using DFC.App.RelatedCareers.Tests.IntegrationTests.API.Support.AzureServiceBus;
using DFC.App.RelatedCareers.Tests.IntegrationTests.API.Support.AzureServiceBus.ServiceBusFactory;
using DFC.App.RelatedCareers.Tests.IntegrationTests.API.Support.AzureServiceBus.ServiceBusFactory.Interface;
using FakeItEasy;
using Microsoft.Azure.ServiceBus;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.RelatedCareers.Tests.TestFramework.UnitTests
{
    public class ServiceBusTests
    {
        private AppSettings appSettings;
        private Message message;
        private IServiceBusSupport serviceBus;
        private ITopicClient topicClient;
        private ITopicClientFactory topicClientFactory;

        public ServiceBusTests()
        {
            this.appSettings = new AppSettings();
            this.topicClient = A.Fake<ITopicClient>();
            this.appSettings.ServiceBusConfig.ConnectionString = "ConnectionString";
            this.message = new Message(Array.Empty<byte>());
            this.topicClientFactory = A.Fake<ITopicClientFactory>();
            A.CallTo(() => this.topicClientFactory.Create(this.appSettings.ServiceBusConfig.ConnectionString)).Returns(this.topicClient);
            this.serviceBus = new ServiceBusSupport(this.topicClientFactory, this.appSettings);
            A.CallTo(() => this.topicClient.SendAsync(this.message)).Returns(Task.CompletedTask);
        }

        [Fact]
        public void OneSendMessageCallIsMadeToTheServiceBusTopicClient()
        {
            this.serviceBus.SendMessage(this.message);
            A.CallTo(() => this.topicClient.SendAsync(this.message)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void AppSettingsConnectionStringIsPassedToTheServiceBusTopicClient()
        {
            this.serviceBus.SendMessage(this.message);
            A.CallTo(() => this.topicClientFactory.Create(this.appSettings.ServiceBusConfig.ConnectionString)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void CreateANewServiceBusMessage()
        {
            IMessageFactory messageFactory = new MessageFactory();
            Message message = messageFactory.Create("id", Array.Empty<byte>(), "action", "content");
            Assert.Equal("id", message.MessageId);
            Assert.Equal(Array.Empty<byte>(), message.Body);
            Assert.Equal("action", message.UserProperties["ActionType"]);
            Assert.Equal("content", message.UserProperties["CType"]);
        }
    }
}
