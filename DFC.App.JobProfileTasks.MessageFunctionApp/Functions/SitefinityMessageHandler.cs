using DFC.App.JobProfileTasks.Data.Constants;
using DFC.App.JobProfileTasks.MessageFunctionApp.Services;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
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
            var thisClassPath = typeof(SitefinityMessageHandler).FullName;

            if (sitefinityMessage != null)
            {
                sitefinityMessage.UserProperties.TryGetValue(MessageProperty.Action, out var messageAction);
                sitefinityMessage.UserProperties.TryGetValue(MessageProperty.ContentType, out var messageContentType);
                sitefinityMessage.UserProperties.TryGetValue(MessageProperty.JobProfileId, out var jobProfileId);

                // loggger should allow setting up correlation id and should be picked up from message
                logger.LogInformation($"{thisClassPath}: Received message action {messageAction} for type {messageContentType} with Id: {jobProfileId}: Correlation id {sitefinityMessage.CorrelationId}");

                var message = Encoding.UTF8.GetString(sitefinityMessage.Body);

                await messageProcessor.ProcessAsync(message, messageAction.ToString(), messageContentType.ToString(), jobProfileId.ToString(), sitefinityMessage.SystemProperties.SequenceNumber).ConfigureAwait(false);
            }
        }
    }
}
