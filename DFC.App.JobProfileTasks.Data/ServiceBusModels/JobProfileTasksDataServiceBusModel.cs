using System;
using System.Collections.Generic;

namespace DFC.App.JobProfileTasks.Data.ServiceBusModels
{
    public class JobProfileTasksDataServiceBusModel
    {
        public DateTime LastReviewed { get; set; }

        public string Introduction { get; set; }

        public string Tasks { get; set; }

        public IEnumerable<JobProfileTasksDataLocationServiceBusModel> Location { get; set; }

        public IEnumerable<JobProfileTasksDataEnvironmentServiceBusModel> Environment { get; set; }

        public IEnumerable<JobProfileTasksDataUniformServiceBusModel> Uniform { get; set; }
    }
}