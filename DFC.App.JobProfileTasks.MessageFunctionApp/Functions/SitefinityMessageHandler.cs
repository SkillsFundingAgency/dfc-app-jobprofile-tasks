using DFC.App.JobProfileTasks.Data.Constants;
using DFC.App.JobProfileTasks.Data.Enums;
using DFC.App.JobProfileTasks.Data.Models.ServiceBusModels;
using DFC.App.JobProfileTasks.MessageFunctionApp.Services;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.MessageFunctionApp.Functions
{
    public class SitefinityMessageHandler
    {
        private readonly IMessageProcessor messageProcessor;
        private readonly ILogger<SitefinityMessageHandler> logger;

        public SitefinityMessageHandler(
            IMessageProcessor messageProcessor,
            ILogger<SitefinityMessageHandler> logger)
        {
            this.messageProcessor = messageProcessor;
            this.logger = logger;
        }

        [FunctionName("SitefinityMessageHandler")]
        public async Task Run([ServiceBusTrigger("%cms-messages-topic%", "%cms-messages-subscription%", Connection = "service-bus-connection-string")] Message sitefinityMessage)
        {
            if (sitefinityMessage != null)
            {
                sitefinityMessage.UserProperties.TryGetValue(MessageProperty.Action, out var messagePropertyActionType);
                sitefinityMessage.UserProperties.TryGetValue(MessageProperty.ContentType, out var messagePropertyContentType);
                sitefinityMessage.UserProperties.TryGetValue(MessageProperty.JobProfileId, out var jobProfileId);

                if (!Enum.TryParse<MessageActionType>(messagePropertyActionType?.ToString(), out var messageActionType))
                {
                    throw new ArgumentOutOfRangeException(nameof(messagePropertyActionType), $"Invalid message action '{messagePropertyActionType}' received, should be one of '{string.Join(",", Enum.GetNames(typeof(MessageActionType)))}'");
                }

                if (!Enum.TryParse<MessageContentType>(messagePropertyContentType?.ToString(), out var messageContentType))
                {
                    throw new ArgumentOutOfRangeException(nameof(messagePropertyContentType), $"Invalid message content type '{messagePropertyContentType}' received, should be one of '{string.Join(",", Enum.GetNames(typeof(MessageContentType)))}'");
                }

                if (!Guid.TryParse(jobProfileId.ToString(), out var jobProfileGuid))
                {
                    throw new InvalidCastException($"Invalid guid received {jobProfileId}");
                }

                var sequenceNumber = sitefinityMessage.SystemProperties.SequenceNumber;

                var messageBody = Encoding.UTF8.GetString(sitefinityMessage?.Body);
                if (string.IsNullOrWhiteSpace(messageBody))
                {
                    throw new ArgumentException("Message cannot be null or empty.", nameof(sitefinityMessage));
                }

                logger.LogInformation($"{nameof(SitefinityMessageHandler)}: Received message action {messagePropertyActionType} for type {messagePropertyContentType} with Id: {jobProfileId}: Correlation id {sitefinityMessage.CorrelationId}");

                var message = Encoding.UTF8.GetString(sitefinityMessage.Body);

                switch (messageActionType)
                {
                    case MessageActionType.Deleted:
                        switch (messageContentType)
                        {
                            case MessageContentType.Uniform:
                                var jobProfileServiceUniformDeleteServiceBusModel = JsonConvert.DeserializeObject<JobProfileUniformDeleteServiceBusModel>(message);
                                await messageProcessor.DeleteUniform(jobProfileGuid, jobProfileServiceUniformDeleteServiceBusModel.UniformId, sequenceNumber).ConfigureAwait(false);
                                break;
                        }

                        break;
                    case MessageActionType.Published:
                        switch (messageContentType)
                        {
                            case MessageContentType.JobProfile:
                                var jobProfileServiceBusModel = JsonConvert.DeserializeObject<JobProfileServiceBusModel>(message);

                                if (jobProfileServiceBusModel == null)
                                {
                                    throw new InvalidOperationException($"Service bus model is null");
                                }

                                await messageProcessor.Save(jobProfileServiceBusModel, messageContentType, jobProfileGuid, sequenceNumber).ConfigureAwait(false);
                                break;
                            case MessageContentType.Uniform:
                                var uniformPatchServiceBusModel = JsonConvert.DeserializeObject<JobProfileTasksDataUniformServiceBusModel>(messageBody);
                                await messageProcessor.PatchUniform(uniformPatchServiceBusModel, jobProfileGuid, sequenceNumber).ConfigureAwait(false);
                                break;
                        }

                        break;
                }
            }
        }
    }
}
