using RestSharp;
using System;

namespace DFC.App.JobProfileTasks.FunctionalTests.Support.API.RestFactory.Interface
{
    public interface IRestClientFactory
    {
        IRestClient Create(Uri baseUrl);
    }
}
