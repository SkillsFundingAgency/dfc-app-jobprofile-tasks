using DFC.App.JobProfileTasks.Data.Contracts;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfileTasks.MessageFunctionApp.Models
{
    public class JobProfileTasksServiceBusModel
    {
        public Guid Id { get; set; }

        [Required]
        public string CanonicalName { get; set; }

        public DateTime LastReviewed { get; set; }

        public string PartitionKey => SocLevelTwo;

        [Required]
        public string SocLevelTwo { get; set; }

        public JobProfileTasksDataServiceBusModel Data { get; set; }
    }
}