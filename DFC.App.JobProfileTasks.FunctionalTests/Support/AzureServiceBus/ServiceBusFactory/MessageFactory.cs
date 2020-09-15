using DFC.App.JobProfileOverview.Tests.IntegrationTests.API.Support.AzureServiceBus.ServiceBusFactory.Interface;
using Microsoft.Azure.ServiceBus;
using System;

namespace DFC.App.JobProfileOverview.Tests.IntegrationTests.API.Support.AzureServiceBus.ServiceBusFactory
{
    public class MessageFactory : IMessageFactory
    {
        public Message Create(string messageId, byte[] body, string actionType, string contentType)
        {
            Message message = new Message()
            {
                ContentType = contentType,
                Body = body,
                CorrelationId = Guid.NewGuid().ToString(),
                Label = "Automated message",
                MessageId = messageId,
            };

            message.UserProperties.Add("ActionType", actionType);
            message.UserProperties.Add("CType", contentType);
            message.UserProperties.Add("Id", messageId);
            return message;
        }
    }
}
