using DFC.App.JobProfileTasks.Common.Contracts;
using DFC.App.JobProfileTasks.MessageFunctionApp.Services;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using System;
using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.MessageFunctionApp.Functions
{
    public class SitefinityMessageHandler
    {
        private readonly IMessagePreProcessor messagePreProcessor;
        private readonly ILogService logService;
        private readonly ICorrelationIdProvider correlationIdProvider;

        public SitefinityMessageHandler(IMessagePreProcessor messagePreProcessor, ILogService logService, ICorrelationIdProvider correlationIdProvider)
        {
            this.messagePreProcessor = messagePreProcessor;
            this.logService = logService;
            this.correlationIdProvider = correlationIdProvider;
        }

        [FunctionName("SitefinityMessageHandler")]
        public async Task Run([ServiceBusTrigger("%cms-messages-topic%", "%cms-messages-subscription%", Connection = "service-bus-connection-string")] Message sitefinityMessage)
        {
            if (sitefinityMessage == null)
            {
                throw new ArgumentNullException(nameof(sitefinityMessage));
            }

            correlationIdProvider.CorrelationId = sitefinityMessage.CorrelationId;

            logService.LogMessage("Received message");
            await messagePreProcessor.Process(sitefinityMessage).ConfigureAwait(false);
            logService.LogMessage("Processed message");
        }
    }
}