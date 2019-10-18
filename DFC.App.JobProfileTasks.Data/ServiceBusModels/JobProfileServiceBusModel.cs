using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfileTasks.Data.ServiceBusModels
{
    public class JobProfileServiceBusModel
    {
        public string CanonicalName { get; set; }

        [Required]
        public string SOCLevelTwo { get; set; }

        public JobProfileTasksDataServiceBusModel WhatYouWillDoData { get; set; }
    }
}