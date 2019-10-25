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
            if (sitefinityMessage == null)
            {
                throw new ArgumentNullException(nameof(sitefinityMessage));
            }

            sitefinityMessage.UserProperties.TryGetValue(MessageProperty.Action, out var messagePropertyActionType);
            sitefinityMessage.UserProperties.TryGetValue(MessageProperty.ContentType, out var messagePropertyContentType);

            if (!Enum.TryParse<MessageActionType>(messagePropertyActionType?.ToString(), out var messageActionType))
            {
                throw new ArgumentOutOfRangeException(nameof(messagePropertyActionType), $"Invalid message action '{messagePropertyActionType}' received, should be one of '{string.Join(",", Enum.GetNames(typeof(MessageActionType)))}'");
            }

            if (!Enum.TryParse<MessageContentType>(messagePropertyContentType?.ToString(), out var messageContentType))
            {
                throw new ArgumentOutOfRangeException(nameof(messagePropertyContentType), $"Invalid message content type '{messagePropertyContentType}' received, should be one of '{string.Join(",", Enum.GetNames(typeof(MessageContentType)))}'");
            }

            var messageBody = Encoding.UTF8.GetString(sitefinityMessage?.Body);
            if (string.IsNullOrWhiteSpace(messageBody))
            {
                throw new ArgumentException("Message cannot be null or empty.", nameof(sitefinityMessage));
            }

            switch (messageActionType)
            {
                case MessageActionType.Deleted:
                    await ProcessDeleted(messageContentType, messageBody).ConfigureAwait(false);
                    break;
                case MessageActionType.Published:
                    var sequenceNumber = sitefinityMessage.SystemProperties.SequenceNumber;
                    await ProcessPublished(messageContentType, messageBody, sequenceNumber).ConfigureAwait(false);
                    break;
            }
        }


        private async Task ProcessDeleted(MessageContentType messageContentType, string messageBody)
        {
            switch (messageContentType)
            {
                case MessageContentType.JobProfile:
                    var jobProfileDeleteServiceBusModel = ConvertOrThrow<JobProfileDeleteServiceBusModel>(messageBody);
                    await messageProcessor.Delete(jobProfileDeleteServiceBusModel.JobProfileId).ConfigureAwait(false);
                    break;
                case MessageContentType.Uniform:
                    var jobProfileServiceUniformDeleteServiceBusModel = ConvertOrThrow<JobProfileUniformDeleteServiceBusModel>(messageBody);
                    await messageProcessor.DeleteUniform(jobProfileServiceUniformDeleteServiceBusModel.JobProfileId, jobProfileServiceUniformDeleteServiceBusModel.Id).ConfigureAwait(false);
                    break;
                case MessageContentType.Location:
                    var jobProfileLocationDeleteServiceBusModel = ConvertOrThrow<JobProfileLocationDeleteServiceBusModel>(messageBody);
                    await messageProcessor.DeleteLocation(jobProfileLocationDeleteServiceBusModel.JobProfileId, jobProfileLocationDeleteServiceBusModel.Id).ConfigureAwait(false);
                    break;
                case MessageContentType.Environment:
                    var jobProfileEnvironmentDeleteServiceBusModel = ConvertOrThrow<JobProfileEnvironmentDeleteServiceBusModel>(messageBody);
                    await messageProcessor.DeleteEnvironment(jobProfileEnvironmentDeleteServiceBusModel.JobProfileId, jobProfileEnvironmentDeleteServiceBusModel.Id).ConfigureAwait(false);
                    break;
            }
        }

        private async Task ProcessPublished(MessageContentType messageContentType, string messageBody, long sequenceNumber)
        {
            switch (messageContentType)
            {
                case MessageContentType.JobProfile:
                    var jobProfileServiceBusModel = ConvertOrThrow<JobProfileServiceBusModel>(messageBody);
                    await messageProcessor.Save(jobProfileServiceBusModel, messageContentType, sequenceNumber).ConfigureAwait(false);
                    break;
                case MessageContentType.Uniform:
                    var uniformPatchServiceBusModel = ConvertOrThrow<JobProfileUniformPatchServiceBusModel>(messageBody);
                    var patchUniformModel = mapper.Map<PatchUniformModel>(uniformPatchServiceBusModel);
                    await messageProcessor.PatchUniform(patchUniformModel).ConfigureAwait(false);
                    break;
                case MessageContentType.Location:
                    var locationPatchServiceBusModel = ConvertOrThrow<JobProfileLocationPatchServiceBusModel>(messageBody);
                    var patchLocationModel = mapper.Map<PatchLocationModel>(locationPatchServiceBusModel);
                    await messageProcessor.PatchLocation(patchLocationModel).ConfigureAwait(false);
                    break;
                case MessageContentType.Environment:
                    var jobProfileEnvironmentPatchServiceBusModel = ConvertOrThrow<JobProfileEnvironmentPatchServiceBusModel>(messageBody);
                    var patchEnvironmentsModel = mapper.Map<PatchEnvironmentsModel>(jobProfileEnvironmentPatchServiceBusModel);
                    await messageProcessor.PatchEnvironment(patchEnvironmentsModel).ConfigureAwait(false);
                    break;
            }
        }

        private T ConvertOrThrow<T>(string messageBody)
        {
            var result = JsonConvert.DeserializeObject<T>(messageBody);
            if (result == null)
            {
                throw new InvalidOperationException($"Message body cannot be converted to {nameof(T)}");
            }

            return result;
        }
    }
}
