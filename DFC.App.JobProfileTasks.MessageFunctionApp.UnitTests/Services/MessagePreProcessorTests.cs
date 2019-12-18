using DFC.App.JobProfileTasks.Common.Contracts;
using DFC.App.JobProfileTasks.Data.Enums;
using DFC.App.JobProfileTasks.MessageFunctionApp.Services;
using FakeItEasy;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileTasks.MessageFunctionApp.UnitTests.Services
{
    public class MessagePreProcessorTests
    {
        private readonly IMessageProcessor processor;
        private readonly ILogService logService;
        private readonly IMessagePropertiesService propertiesService;

        public MessagePreProcessorTests()
        {
            processor = A.Fake<IMessageProcessor>();
            logService = A.Fake<ILogService>();
            propertiesService = A.Fake<IMessagePropertiesService>();
        }

        public static IEnumerable<object[]> ValidStatusCodes => new List<object[]>
        {
            new object[] { HttpStatusCode.OK },
            new object[] { HttpStatusCode.Created },
            new object[] { HttpStatusCode.AlreadyReported },
        };

        [Fact]
        public async Task RunHandlerThrowsArgumentNullExceptionWhenMessageIsNull()
        {
            // Arrange
            var preProcessor = new MessagePreProcessor(processor, logService, propertiesService);

            // Act
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await preProcessor.Process(null).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact]
        public async Task RunHandlerThrowsArgumentExceptionWhenMessageBodyIsEmpty()
        {
            // Arrange
            var message = new Message(Encoding.ASCII.GetBytes(string.Empty));
            var preProcessor = new MessagePreProcessor(processor, logService, propertiesService);

            // Act
            await Assert.ThrowsAsync<ArgumentException>(async () => await preProcessor.Process(message).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact]
        public async Task RunHandlerThrowsArgumentOutOfRangeExceptionWhenMessageActionIsInvalid()
        {
            // Arrange
            var message = CreateBaseMessage(messageAction: (MessageActionType)999);
            var preProcessor = new MessagePreProcessor(processor, logService, propertiesService);

            // Act
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await preProcessor.Process(message).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact]
        public async Task RunHandlerThrowsArgumentOutOfRangeExceptionWhenMessageContentTypeIsInvalid()
        {
            // Arrange
            var message = CreateBaseMessage(contentType: (MessageContentType)999);
            var preProcessor = new MessagePreProcessor(processor, logService, propertiesService);

            // Act
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await preProcessor.Process(message).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact]
        public async Task RunHandlerLogsWarningWhenMessageProcessorReturnsError()
        {
            // Arrange
            var message = CreateBaseMessage();
            var fakeProcessor = A.Fake<IMessageProcessor>();
            A.CallTo(() => fakeProcessor.ProcessAsync(A<string>.Ignored, A<long>.Ignored, A<MessageContentType>.Ignored, A<MessageActionType>.Ignored)).Returns(HttpStatusCode.InternalServerError);
            var messagePropertiesService = A.Fake<IMessagePropertiesService>();
            A.CallTo(() => messagePropertiesService.GetSequenceNumber(A<Message>.Ignored)).Returns(123);

            var preProcessor = new MessagePreProcessor(fakeProcessor, logService, propertiesService);

            // Act
            await preProcessor.Process(message).ConfigureAwait(false);

            // Assert
            A.CallTo(() => logService.LogMessage(A<string>.Ignored, SeverityLevel.Warning)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [MemberData(nameof(ValidStatusCodes))]
        public async Task RunHandlerLogsInformationWhenMessageProcessorReturnsSuccess(HttpStatusCode status)
        {
            // Arrange
            var message = CreateBaseMessage();
            var fakeProcessor = A.Fake<IMessageProcessor>();
            A.CallTo(() => fakeProcessor.ProcessAsync(A<string>.Ignored, A<long>.Ignored, A<MessageContentType>.Ignored, A<MessageActionType>.Ignored)).Returns(status);
            var messagePropertiesService = A.Fake<IMessagePropertiesService>();
            A.CallTo(() => messagePropertiesService.GetSequenceNumber(A<Message>.Ignored)).Returns(123);

            var preProcessor = new MessagePreProcessor(fakeProcessor, logService, propertiesService);

            // Act
            await preProcessor.Process(message).ConfigureAwait(false);

            // Assert
            A.CallTo(() => logService.LogMessage(A<string>.Ignored, SeverityLevel.Information)).MustHaveHappenedOnceExactly();
        }

        private Message CreateBaseMessage(MessageActionType messageAction = MessageActionType.Published, MessageContentType contentType = MessageContentType.JobProfile)
        {
            var message = A.Fake<Message>();
            message.Body = Encoding.ASCII.GetBytes("Some body json object here");
            message.UserProperties.Add("ActionType", messageAction.ToString());
            message.UserProperties.Add("CType", contentType);

            return message;
        }
    }
}