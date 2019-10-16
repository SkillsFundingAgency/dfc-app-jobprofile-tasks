using System;

namespace DFC.App.JobProfileTasks.MessageFunctionApp.HttpClientPolicies
{
    public class JobProfileClientOptions
    {
        public Uri BaseAddress { get; set; }

        public TimeSpan Timeout { get; set; } = new TimeSpan(0, 0, 10);
    }
}