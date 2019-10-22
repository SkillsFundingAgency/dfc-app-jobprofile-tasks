using AutoMapper;
using DFC.App.JobProfileTasks.Data.Constants;
using DFC.App.JobProfileTasks.Data.Enums;
using DFC.App.JobProfileTasks.Data.Models.PatchModels;
using DFC.App.JobProfileTasks.Data.Models.ServiceBusModels;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.MessageFunctionApp.Services
{
    public class MessagePreProcessor : IMessagePreProcessor
    {
        private readonly IMessageProcessor messageProcessor;
        private readonly ILogger<MessagePreProcessor> logger;
        private readonly IMapper mapper;

        public MessagePreProcessor(IMessageProcessor messageProcessor, ILogger<MessagePreProcessor> logger, IMapper mapper)
        {
            this.messageProcessor = messageProcessor;
            this.logger = logger;
            this.mapper = mapper;
        }

        public async Task Process(Message sitefinityMessage)
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

            if (jobProfileId == null)
            {
                throw new ArgumentOutOfRangeException($"No job profile Id specified");
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
                            var uniformPatchServiceBusModel = JsonConvert.DeserializeObject<JobProfileUniformPatchServiceBusModel>(messageBody);
                            var patchUniformModel = mapper.Map<PatchUniformModel>(uniformPatchServiceBusModel);
                            patchUniformModel.JobProfileId = jobProfileGuid;
                            await messageProcessor.PatchUniform(patchUniformModel, jobProfileGuid, sequenceNumber).ConfigureAwait(false);
                            break;
                        case MessageContentType.Location:
                            var locationPatchServiceBusModel = JsonConvert.DeserializeObject<JobProfileLocationPatchServiceBusModel>(messageBody);
                            var patchLocationModel = mapper.Map<PatchLocationModel>(locationPatchServiceBusModel);
                            patchLocationModel.JobProfileId = jobProfileGuid;
                            await messageProcessor.PatchLocation(patchLocationModel, jobProfileGuid, sequenceNumber).ConfigureAwait(false);
                            break;
                        case MessageContentType.Environment:
                            var jobProfileEnvironmentPatchServiceBusModel = JsonConvert.DeserializeObject<JobProfileEnvironmentPatchServiceBusModel>(messageBody);
                            var patchEnvironmentsModel = mapper.Map<PatchEnvironmentsModel>(jobProfileEnvironmentPatchServiceBusModel);
                            patchEnvironmentsModel.JobProfileId = jobProfileGuid;
                            await messageProcessor.PatchEnvironment(patchEnvironmentsModel, jobProfileGuid, sequenceNumber).ConfigureAwait(false);
                            break;
                    }

                    break;
            }
        }
    }
}
