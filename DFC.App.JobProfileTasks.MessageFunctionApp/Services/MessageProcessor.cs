using AutoMapper;
using DFC.App.JobProfileTasks.Data.Enums;
using DFC.App.JobProfileTasks.Data.Models.PatchModels;
using DFC.App.JobProfileTasks.Data.Models.ServiceBusModels.PatchModels;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.MessageFunctionApp.Services
{
    public class MessageProcessor : IMessageProcessor
    {
        private readonly IMapper mapper;
        private readonly IHttpClientService httpClientService;
        private readonly IMappingService mappingService;

        public MessageProcessor(IMapper mapper, IHttpClientService httpClientService, IMappingService mappingService)
        {
            this.mapper = mapper;
            this.httpClientService = httpClientService;
            this.mappingService = mappingService;
        }

        public async Task<HttpStatusCode> ProcessAsync(string message, long sequenceNumber, MessageContentType messageContentType, MessageActionType messageAction)
        {
            switch (messageContentType)
            {
                case MessageContentType.Environment:
                    {
                        var serviceBusMessage = JsonConvert.DeserializeObject<PatchEnvironmentServiceBusModel>(message);
                        var patchEnvironmentsModel = mapper.Map<PatchEnvironmentsModel>(serviceBusMessage);
                        patchEnvironmentsModel.MessageAction = messageAction;
                        patchEnvironmentsModel.SequenceNumber = sequenceNumber;

                        return await httpClientService.PatchAsync(patchEnvironmentsModel, "environment").ConfigureAwait(false);
                    }

                case MessageContentType.Location:
                    {
                        var serviceBusMessage = JsonConvert.DeserializeObject<PatchLocationServiceBusModel>(message);
                        var patchLocationModel = mapper.Map<PatchLocationModel>(serviceBusMessage);
                        patchLocationModel.MessageAction = messageAction;
                        patchLocationModel.SequenceNumber = sequenceNumber;

                        return await httpClientService.PatchAsync(patchLocationModel, "location").ConfigureAwait(false);
                    }

                case MessageContentType.Uniform:
                    {
                        var serviceBusMessage = JsonConvert.DeserializeObject<PatchUniformServiceBusModel>(message);
                        var patchUniformModel = mapper.Map<PatchUniformModel>(serviceBusMessage);
                        patchUniformModel.MessageAction = messageAction;
                        patchUniformModel.SequenceNumber = sequenceNumber;

                        return await httpClientService.PatchAsync(patchUniformModel, "uniform").ConfigureAwait(false);
                    }

                case MessageContentType.JobProfile:
                    return await ProcessJobProfileMessageAsync(message, messageAction, sequenceNumber).ConfigureAwait(false);

                default:
                    break;
            }

            return await Task.FromResult(HttpStatusCode.InternalServerError).ConfigureAwait(false);
        }

        private async Task<HttpStatusCode> ProcessJobProfileMessageAsync(string message, MessageActionType messageAction, long sequenceNumber)
        {
            var jobProfile = mappingService.MapToSegmentModel(message, sequenceNumber);

            switch (messageAction)
            {
                case MessageActionType.Draft:
                case MessageActionType.Published:
                    var result = await httpClientService.PutAsync(jobProfile).ConfigureAwait(false);
                    if (result == HttpStatusCode.NotFound)
                    {
                        return await httpClientService.PostAsync(jobProfile).ConfigureAwait(false);
                    }

                    return result;

                case MessageActionType.Deleted:
                    return await httpClientService.DeleteAsync(jobProfile.DocumentId).ConfigureAwait(false);

                default:
                    throw new ArgumentOutOfRangeException(nameof(messageAction), $"Invalid message action '{messageAction}' received, should be one of '{string.Join(",", Enum.GetNames(typeof(MessageActionType)))}'");
            }
        }
    }
}