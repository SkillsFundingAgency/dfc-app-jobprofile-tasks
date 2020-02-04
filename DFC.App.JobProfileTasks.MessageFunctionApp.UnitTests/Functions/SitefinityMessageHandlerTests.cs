using DFC.App.JobProfileTasks.Data.Models.PatchModels;
using DFC.App.JobProfileTasks.MessageFunctionApp.Functions;
using DFC.App.JobProfileTasks.MessageFunctionApp.Services;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileTasks.MessageFunctionApp.UnitTests.Functions
{
    public class SitefinityMessageHandlerTests
    {
        private readonly IMessagePreProcessor processor;
        private readonly ICorrelationIdProvider correlationIdProvider;
        private readonly ILogService logger;

        public SitefinityMessageHandlerTests()
        {
            processor = A.Fake<IMessagePreProcessor>();
            correlationIdProvider = A.Fake<ICorrelationIdProvider>();
            logger = A.Fake<ILogService>();
        }

        [Fact]
        public async Task RunHandlerThrowsArgumentNullExceptionWhenMessageIsNull()
        {
            // Arrange
            var handler = new SitefinityMessageHandler(processor, logger, correlationIdProvider);

            // Act
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await handler.Run(null).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact]
        public async Task RunHandlerCallsMessagePreProcessorWhenMessageIsNotNull()
        {
            // Arrange
            var model = A.Fake<PatchEnvironmentsModel>();
            var message = JsonConvert.SerializeObject(model);
            var serviceBusMessage = new Message(Encoding.ASCII.GetBytes(message));

            var handler = new SitefinityMessageHandler(processor, logger, correlationIdProvider);

            // Act
            await handler.Run(serviceBusMessage).ConfigureAwait(false);

            A.CallTo(() => processor.Process(A<Message>.Ignored)).MustHaveHappenedOnceExactly();
        }
    }
}