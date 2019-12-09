using System;
using System.Collections.Generic;

namespace DFC.App.JobProfileTasks.Data.Models.ServiceBusModels
{
    public class JobProfileTasksDataServiceBusModel
    {
        public DateTime LastReviewed { get; set; }

        public string Introduction { get; set; }

        public string DailyTasks { get; set; }

        public IEnumerable<Location> Locations { get; set; }

        public IEnumerable<Uniform> Uniforms { get; set; }

        public IEnumerable<Environment> Environments { get; set; }
    }
}