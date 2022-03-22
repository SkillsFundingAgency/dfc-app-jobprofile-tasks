using DFC.App.JobProfileTasks.Data.Models.PatchModels;
using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using DFC.App.JobProfileTasks.MessageFunctionApp.HttpClientPolicies;
using DFC.Logger.AppInsights.Constants;
using DFC.Logger.AppInsights.Contracts;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Mime;
using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.MessageFunctionApp.Services
{
    public class HttpClientService : IHttpClientService
    {
        private readonly JobProfileClientOptions jobProfileClientOptions;
        private readonly HttpClient httpClient;
        private readonly ILogService logger;
        private readonly ICorrelationIdProvider correlationIdProvider;

        public HttpClientService(JobProfileClientOptions jobProfileClientOptions, IHttpClientFactory httpClientFactory, ILogService logger, ICorrelationIdProvider correlationIdProvider)
        {
            this.jobProfileClientOptions = jobProfileClientOptions;
            this.httpClient = httpClientFactory.CreateClient();
            this.logger = logger;
            this.correlationIdProvider = correlationIdProvider;
        }

        public async Task<HttpStatusCode> PostAsync(JobProfileTasksSegmentModel tasksSegmentModel)
        {
            var url = new Uri($"{jobProfileClientOptions?.BaseAddress}segment");

            using (var content = new ObjectContent(typeof(JobProfileTasksSegmentModel), tasksSegmentModel, new JsonMediaTypeFormatter(), MediaTypeNames.Application.Json))
            {
                ConfigureHttpClient();
                var response = await httpClient.PostAsync(url, content).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    logger.LogError($"Failure status code '{response.StatusCode}' received with content '{responseContent}', for POST, Id: {tasksSegmentModel?.DocumentId}.");
                    response.EnsureSuccessStatusCode();
                }

                return response.StatusCode;
            }
        }

        public async Task<HttpStatusCode> PutAsync(JobProfileTasksSegmentModel tasksSegmentModel)
        {
            var url = new Uri($"{jobProfileClientOptions?.BaseAddress}segment");

            using (var content = new ObjectContent(typeof(JobProfileTasksSegmentModel), tasksSegmentModel, new JsonMediaTypeFormatter(), MediaTypeNames.Application.Json))
            {
                ConfigureHttpClient();
                var response = await httpClient.PutAsync(url, content).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotFound)
                {
                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    logger.LogError($"Failure status code '{response.StatusCode}' received with content '{responseContent}', for Put type {typeof(JobProfileTasksSegmentModel)}, Id: {tasksSegmentModel?.DocumentId}");
                    response.EnsureSuccessStatusCode();
                }

                return response.StatusCode;
            }
        }

        public async Task<HttpStatusCode> PatchAsync<T>(T patchModel, string patchTypeEndpoint)
            where T : BasePatchModel
        {
            var url = new Uri($"{jobProfileClientOptions.BaseAddress}segment/{patchModel?.JobProfileId}/{patchTypeEndpoint}");
            ConfigureHttpClient();

            using (var content = new ObjectContent<T>(patchModel, new JsonMediaTypeFormatter(), MediaTypeNames.Application.Json))
            {
                var response = await httpClient.PatchAsync(url, content).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotFound)
                {
                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    logger.LogError($"Failure status code '{response.StatusCode}' received with content '{responseContent}', for patch type {typeof(T)}, Id: {patchModel?.JobProfileId}");
                    response.EnsureSuccessStatusCode();
                }

                return response.StatusCode;
            }
        }

        public async Task<HttpStatusCode> DeleteAsync(Guid id)
        {
            var url = new Uri($"{jobProfileClientOptions?.BaseAddress}segment/{id}");
            ConfigureHttpClient();
            var response = await httpClient.DeleteAsync(url).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotFound)
            {
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                logger.LogError($"Failure status code '{response.StatusCode}' received with content '{responseContent}', for DELETE, Id: {id}.");
                response.EnsureSuccessStatusCode();
            }

            return response.StatusCode;
        }

        private void ConfigureHttpClient()
        {
            if (!httpClient.DefaultRequestHeaders.Contains(HeaderName.CorrelationId))
            {
                httpClient.DefaultRequestHeaders.Add(HeaderName.CorrelationId, correlationIdProvider.CorrelationId);
            }
        }
    }
}