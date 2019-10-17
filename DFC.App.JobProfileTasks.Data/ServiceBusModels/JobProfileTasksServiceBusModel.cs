using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfileTasks.Data.ServiceBusModels
{
    public class JobProfileTasksServiceBusModel
    {
        public Guid Id { get; set; }

        [Required]
        public string CanonicalName { get; set; }

        [Required]
        public string SocLevelTwo { get; set; }

        public JobProfileTasksDataServiceBusModel Data { get; set; }
    }
}