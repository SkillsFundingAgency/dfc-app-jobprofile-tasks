using System;

namespace DFC.App.JobProfileTasks.Data.Models.ServiceBusModels
{
    public class JobProfileTasksDataUniformServiceBusModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsNegative { get; set; }
    }
}
