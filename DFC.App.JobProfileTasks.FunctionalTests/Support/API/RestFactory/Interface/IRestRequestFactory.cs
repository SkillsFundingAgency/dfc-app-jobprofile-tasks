using RestSharp;
using System;

namespace DFC.App.JobProfileTasks.FunctionalTests.Support.API.RestFactory.Interface
{
    public interface IRestRequestFactory
    {
        IRestRequest Create(string urlSuffix);
    }
}
