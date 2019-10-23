using System.Collections.Generic;

namespace DFC.App.JobProfileTasks.Data.Models.ServiceBusModels
{
    public class JobProfileTasksDataServiceBusModel
    {
        public bool IsCadReady { get; set; }

        public string Introduction { get; set; }

        public string DailyTasks { get; set; }

        public IEnumerable<JobProfileTasksDataLocationServiceBusModel> Locations { get; set; }

        public IEnumerable<JobProfileTasksDataUniformServiceBusModel> Uniforms { get; set; }

        public IEnumerable<JobProfileTasksDataEnvironmentServiceBusModel> Environments { get; set; }
    }
}