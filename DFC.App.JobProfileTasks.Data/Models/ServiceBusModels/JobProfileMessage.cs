﻿using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfileTasks.Data.Models.ServiceBusModels
{
    public class JobProfileMessage : BaseJobProfileMessage
    {
        public string CanonicalName { get; set; }

        public DateTime LastModified { get; set; }

        [Required]
        public string SOCLevelTwo { get; set; }

        public JobProfileTasksDataServiceBusModel WhatYouWillDoData { get; set; }
    }
}