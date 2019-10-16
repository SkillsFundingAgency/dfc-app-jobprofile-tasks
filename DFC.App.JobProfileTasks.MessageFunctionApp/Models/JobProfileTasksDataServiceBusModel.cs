using System;
using System.Collections.Generic;

namespace DFC.App.JobProfileTasks.MessageFunctionApp.Models
{
    public class JobProfileTasksDataServiceBusModel
    {
        public DateTime LastReviewed { get; set; }

        public string Introduction { get; set; }

        public string Tasks { get; set; }

        public IEnumerable<JobProfileTasksDataLocationServiceBusModel> Locations { get; set; }

        public IEnumerable<JobProfileTasksDataEnvironmentServiceBusModel> Environments { get; set; }

        public IEnumerable<JobProfileTasksDataUniformServiceBusModel> Uniforms { get; set; }
    }
}