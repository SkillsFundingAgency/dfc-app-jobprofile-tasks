using System;

namespace DFC.App.JobProfileTasks.Data.Models.ServiceBusModels
{
    public class JobProfileDeleteBaseServiceBusModel
    {
        public Guid JobProfileId { get; set; }

        public Guid Id { get; set; }
    }
}
