using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfileTasks.Data.Models.ServiceBusModels
{
    public class JobProfileServiceBusModel
    {
        public Guid JobProfileId { get; set; }

        public string CanonicalName { get; set; }

        [Required]
        public string SOCLevelTwo { get; set; }

        public DateTime LastModified { get; set; }

        public JobProfileTasksDataServiceBusModel WhatYouWillDoData { get; set; }
    }
}