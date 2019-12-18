using DFC.App.JobProfileTasks.Common.Contracts;
using DFC.App.JobProfileTasks.Data.Constants;
using DFC.App.JobProfileTasks.Data.Enums;
using DFC.App.JobProfileTasks.MessageFunctionApp.Functions;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.ServiceBus;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.MessageFunctionApp.Services
{
    public class MessagePreProcessor : IMessagePreProcessor
    {
        private static readonly string ClassFullName = typeof(SitefinityMessageHandler).FullName;
        private readonly IMessageProcessor messageProcessor;
        private readonly ILogService logService;
        private readonly IMessagePropertiesService messagePropertiesService;

        public MessagePreProcessor(IMessageProcessor messageProcessor, ILogService logService, IMessagePropertiesService messagePropertiesService)
        {
            this.messageProcessor = messageProcessor;
            this.logService = logService;
            this.messagePropertiesService = messagePropertiesService;
        }

        public async Task Process(Message sitefinityMessage)
        {
            if (sitefinityMessage == null)
            {
                throw new ArgumentNullException(nameof(sitefinityMessage));
            }

            sitefinityMessage.UserProperties.TryGetValue(MessageProperty.Action, out var messagePropertyActionType);
            sitefinityMessage.UserProperties.TryGetValue(MessageProperty.ContentType, out var messagePropertyContentType);
            sitefinityMessage.UserProperties.TryGetValue(MessageProperty.JobProfileId, out var messageContentId);

            var messageBody = Encoding.UTF8.GetString(sitefinityMessage?.Body);
            if (string.IsNullOrWhiteSpace(messageBody))
            {
                throw new ArgumentException("Message cannot be null or empty.", nameof(sitefinityMessage));
            }

            if (!Enum.IsDefined(typeof(MessageActionType), messagePropertyActionType?.ToString()))
            {
                throw new ArgumentOutOfRangeException(nameof(messagePropertyActionType), $"Invalid message action '{messagePropertyActionType}' received, should be one of '{string.Join(",", Enum.GetNames(typeof(MessageActionType)))}'");
            }

            if (!Enum.IsDefined(typeof(MessageContentType), messagePropertyContentType?.ToString()))
            {
                throw new ArgumentOutOfRangeException(nameof(messagePropertyContentType), $"Invalid message content type '{messagePropertyContentType}' received, should be one of '{string.Join(",", Enum.GetNames(typeof(MessageContentType)))}'");
            }

            var messageAction = Enum.Parse<MessageActionType>(messagePropertyActionType?.ToString());
            var messageContentType = Enum.Parse<MessageContentType>(messagePropertyContentType?.ToString());
            var sequenceNumber = messagePropertiesService.GetSequenceNumber(sitefinityMessage);

            var result = await messageProcessor.ProcessAsync(messageBody, sequenceNumber, messageContentType, messageAction).ConfigureAwait(false);

            switch (result)
            {
                case HttpStatusCode.OK:
                    logService.LogMessage($"{ClassFullName}: JobProfile Id: {messageContentId}: Updated segment");
                    break;

                case HttpStatusCode.Created:
                    logService.LogMessage($"{ClassFullName}: JobProfile Id: {messageContentId}: Created segment");
                    break;

                case HttpStatusCode.AlreadyReported:
                    logService.LogMessage($"{ClassFullName}: JobProfile Id: {messageContentId}: Segment previously updated");
                    break;

                default:
                    logService.LogMessage($"{ClassFullName}: JobProfile Id: {messageContentId}: Segment not Posted: Status: {result}", SeverityLevel.Warning);
                    break;
            }
        }
    }
}