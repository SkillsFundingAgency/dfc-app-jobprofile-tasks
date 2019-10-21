using AutoMapper;
using DFC.App.JobProfileTasks.Data.Enums;
using DFC.App.JobProfileTasks.Data.Models;
using DFC.App.JobProfileTasks.Data.ServiceBusModels;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
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

        public async Task<HttpStatusCode> Save(
            JobProfileServiceBusModel jobProfileServiceBusModel,
            MessageContentType messageContentType,
            Guid jobProfileId,
            long sequenceNumber)
        {
            var model = mapper.Map<JobProfileTasksSegmentModel>(jobProfileServiceBusModel);
            model.DocumentId = jobProfileId;
            model.SequenceNumber = sequenceNumber;

            var response = await Update(model).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                response = await Create(model).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }

            return response.StatusCode;
        }

        public async Task<HttpStatusCode> Delete(Guid jobProfileId)
        {
            var uri = string.Concat(httpClient.BaseAddress, "segment/", jobProfileId);
            var response = await httpClient.DeleteAsync(uri).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return response.StatusCode;
        }

        public async Task<HttpStatusCode> PatchUniform(JobProfileTasksDataUniformServiceBusModel message, Guid jobProfileId, MessageActionType messageActionType, long sequenceNumber)
        {
            var patchDocument = new JsonPatchDocument<JobProfileTasksDataServiceBusModel>();

            if (messageActionType == MessageActionType.Deleted)
            {

            }

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var content = new StringContent(serialized, Encoding.UTF8, MediaTypeNames.Application.Json);
            var uri = string.Concat(httpClient.BaseAddress, "segment/", jobProfileId);
            var response = await httpClient.PatchAsync(uri, content).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            return response.StatusCode;
        }

        private async Task<HttpResponseMessage> Create(JobProfileTasksSegmentModel model)
        {
            var uri = string.Concat(httpClient.BaseAddress, "segment");
            return await httpClient.PostAsJsonAsync(uri, model).ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> Update(JobProfileTasksSegmentModel model)
        {
            var uri = string.Concat(httpClient.BaseAddress, "segment");
            return await httpClient.PutAsJsonAsync(uri, model).ConfigureAwait(false);
        }
    }
}
