using System;

namespace DFC.App.JobProfileTasks.Data.Models.ServiceBusModels
{
    public class JobProfileBasePatchServiceBusModel
    {
        public Guid JobProfileId { get; set; }

        public Guid Id { get; set; }

        public string Description { get; set; }
    }
}
