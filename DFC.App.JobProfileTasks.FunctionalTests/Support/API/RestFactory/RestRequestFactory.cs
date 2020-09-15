using DFC.App.JobProfileTasks.FunctionalTests.Support.API.RestFactory.Interface;
using RestSharp;

namespace DFC.App.JobProfileTasks.FunctionalTests.Support.API.RestFactory
{
    internal class RestRequestFactory : IRestRequestFactory
    {
        public IRestRequest Create(string urlSuffix)
        {
            return new RestRequest(urlSuffix);
        }
    }
}