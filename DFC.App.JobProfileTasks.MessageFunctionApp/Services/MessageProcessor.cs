using AutoMapper;
using DFC.App.JobProfileTasks.Data.Constants;
using DFC.App.JobProfileTasks.Data.Models;
using DFC.App.JobProfileTasks.MessageFunctionApp.Models;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.MessageFunctionApp.Services
{
    public class MessageProcessor : IMessageProcessor
    {
        private readonly HttpClient httpClient;
        private readonly IMapper mapper;

        public MessageProcessor(IHttpClientFactory httpClientFactory, IMapper mapper)
        {
            this.httpClient = httpClientFactory.CreateClient(nameof(MessageProcessor));
            this.mapper = mapper;
        }

        public async Task<HttpStatusCode> ProcessAsync(
            string message,
            string messageAction,
            string messageContentType,
            string jobProfileId,
            long sequenceNumber)
        {
            switch (messageAction)
            {
                case MessageAction.Delete:
                    return await Delete(jobProfileId).ConfigureAwait(false);
                case MessageAction.Save:
                    var jobProfileMessageModel = JsonConvert.DeserializeObject<JobProfileTasksServiceBusModel>(message);
                    var jobProfileMessageData = mapper.Map<JobProfileTasksDataSegmentModel>(jobProfileMessageModel);
                    return await Save(jobProfileMessageData).ConfigureAwait(false);
            }

            throw new ArgumentOutOfRangeException(nameof(messageAction));
        }

        private async Task<HttpStatusCode> Delete(string jobProfileId)
        {
            var uri = string.Concat(httpClient.BaseAddress, "segment/", jobProfileId);
            var response = await httpClient.DeleteAsync(uri).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return response.StatusCode;
        }

        private async Task<HttpStatusCode> Save(JobProfileTasksDataSegmentModel model)
        {
            var uri = string.Concat(httpClient.BaseAddress, "segment");
            var response = await httpClient.PostAsJsonAsync(uri, model).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return response.StatusCode;
        }
    }
}
