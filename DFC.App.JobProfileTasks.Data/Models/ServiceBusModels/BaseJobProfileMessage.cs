using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfileTasks.Data.Models.ServiceBusModels
{
    public class BaseJobProfileMessage
    {
        [Required]
        public Guid JobProfileId { get; set; }
    }
}