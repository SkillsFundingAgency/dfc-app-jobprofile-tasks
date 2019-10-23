﻿using AutoMapper;
using DFC.App.JobProfileTasks.Data.Enums;
using DFC.App.JobProfileTasks.Data.Models.PatchModels;
using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using DFC.App.JobProfileTasks.Data.Models.ServiceBusModels;
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

        public async Task<HttpStatusCode> DeleteUniform(Guid jobProfileId, Guid uniformId, long sequenceNumber)
        {
            var uri = $"segment/{jobProfileId}/uniform/{uniformId}";
            return await Patch(uri).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> DeleteEnvironment(Guid jobProfileId, Guid environmentId, long sequenceNumber)
        {
            var uri = $"segment/{jobProfileId}/environment/{environmentId}";
            return await Patch(uri).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> DeleteLocation(Guid jobProfileId, Guid locationId, long sequenceNumber)
        {
            var uri = $"segment/{jobProfileId}/location/{locationId}";
            return await Patch(uri).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> PatchUniform(PatchUniformModel message, Guid jobProfileId, long sequenceNumber)
        {
            var url = "segment/uniform";
            return await Patch(message, url).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> PatchLocation(PatchLocationModel message, Guid jobProfileId, long sequenceNumber)
        {
            var url = "segment/location";
            return await Patch(message, url).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> PatchEnvironment(PatchEnvironmentsModel message, Guid jobProfileId, long sequenceNumber)
        {
            var url = "segment/environment";
            return await Patch(message, url).ConfigureAwait(false);
        }

        private async Task<HttpStatusCode> Patch(object message, string url)
        {
            var serialized = JsonConvert.SerializeObject(message);
            var content = new StringContent(serialized, Encoding.UTF8, MediaTypeNames.Application.Json);
            var uri = string.Concat(httpClient.BaseAddress, url);

            var response = await httpClient.PatchAsync(uri, content).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                await httpClient.PostAsync(uri, content).ConfigureAwait(false);
            }

            return response.StatusCode;
        }

        private async Task<HttpStatusCode> Patch(string url)
        {
            var content = new StringContent(string.Empty, Encoding.UTF8, MediaTypeNames.Application.Json);
            var uri = string.Concat(httpClient.BaseAddress, url);

            var response = await httpClient.PatchAsync(uri, content).ConfigureAwait(false);

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
