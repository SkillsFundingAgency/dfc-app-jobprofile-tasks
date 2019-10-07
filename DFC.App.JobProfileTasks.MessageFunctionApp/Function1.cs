using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace DFC.App.JobProfileTasks.MessageFunctionApp
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([ServiceBusTrigger("%dummy-segment-topic-name%", "%dummy-segment-subscription-name%", Connection = "service-bus-connection-string")] string serviceBusMessage, ILogger log)
        {
            log.LogInformation($"C# ServiceBus topic trigger function processed message: {serviceBusMessage}");
        }
    }
}