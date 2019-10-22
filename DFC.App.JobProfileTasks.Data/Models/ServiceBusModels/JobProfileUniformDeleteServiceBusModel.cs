using System;

namespace DFC.App.JobProfileTasks.Data.Models.ServiceBusModels
{
    public class JobProfileUniformDeleteServiceBusModel
    {
        public Guid JobProfileId { get; set; }

        public Guid UniformId { get; set; }
    }
}
