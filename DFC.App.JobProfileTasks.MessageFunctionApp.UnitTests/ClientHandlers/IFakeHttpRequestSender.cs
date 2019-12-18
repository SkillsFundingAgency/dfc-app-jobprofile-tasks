using System.Net.Http;

namespace DFC.App.JobProfileTasks.MessageFunctionApp.UnitTests.ClientHandlers
{
    public interface IFakeHttpRequestSender
    {
        HttpResponseMessage Send(HttpRequestMessage request);
    }
}