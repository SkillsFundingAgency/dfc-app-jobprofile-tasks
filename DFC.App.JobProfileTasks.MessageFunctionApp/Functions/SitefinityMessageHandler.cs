using DFC.App.JobProfileTasks.MessageFunctionApp.Services;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.MessageFunctionApp.Functions
{
    public class SitefinityMessageHandler
    {
        private readonly IMessagePreProcessor messagePreProcessor;
        private readonly ILogger<SitefinityMessageHandler> logger;

        public SitefinityMessageHandler(IMessagePreProcessor messagePreProcessor, ILogger<SitefinityMessageHandler> logger)
        {
            this.messagePreProcessor = messagePreProcessor;
            this.logger = logger;
        }

        [FunctionName("SitefinityMessageHandler")]
        public async Task Run([ServiceBusTrigger("%cms-messages-topic%", "%cms-messages-subscription%", Connection = "service-bus-connection-string")] Message sitefinityMessage)
        {
            if (sitefinityMessage == null)
            {
                logger.LogInformation("Received null message");
            }

            logger.LogInformation("Received message");
            await messagePreProcessor.Process(sitefinityMessage).ConfigureAwait(false);
            logger.LogInformation("Processed message");
        }
    }
}
