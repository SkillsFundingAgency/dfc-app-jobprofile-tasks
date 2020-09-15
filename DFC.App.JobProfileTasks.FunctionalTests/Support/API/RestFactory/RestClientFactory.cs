using DFC.App.JobProfileTasks.FunctionalTests.Support.API.RestFactory.Interface;
using RestSharp;
using System;

namespace DFC.App.JobProfileTasks.FunctionalTests.Support.API.RestFactory
{
    internal class RestClientFactory : IRestClientFactory
    {
        public IRestClient Create(Uri baseUrl)
        {
            return new RestClient(baseUrl);
        }
    }
}
