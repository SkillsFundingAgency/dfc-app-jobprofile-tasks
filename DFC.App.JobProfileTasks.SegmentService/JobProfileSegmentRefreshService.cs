using DFC.Logger.AppInsights.Contracts;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.SegmentService
{
    public class JobProfileSegmentRefreshService<TModel> : IJobProfileSegmentRefreshService<TModel>
    {
        private readonly ITopicClient topicClient;
        private readonly ICorrelationIdProvider correlationIdProvider;

        public JobProfileSegmentRefreshService(ITopicClient topicClient, ICorrelationIdProvider correlationIdProvider)
        {
            this.topicClient = topicClient;
            this.correlationIdProvider = correlationIdProvider;
        }

        public async Task SendMessageAsync(TModel model)
        {
            var messageJson = JsonConvert.SerializeObject(model);
            var message = new Message(Encoding.UTF8.GetBytes(messageJson))
            {
                CorrelationId = correlationIdProvider.CorrelationId,
            };

            await topicClient.SendAsync(message).ConfigureAwait(false);
        }

        public async Task SendMessageListAsync(IList<TModel> models)
        {
            if (models != null)
            {
                foreach (var model in models)
                {
                    var messageJson = JsonConvert.SerializeObject(model);
                    var message = new Message(Encoding.UTF8.GetBytes(messageJson))
                    {
                        CorrelationId = correlationIdProvider.CorrelationId,
                    };

                    await topicClient.SendAsync(message).ConfigureAwait(false);
                }
            }
        }
    }
}