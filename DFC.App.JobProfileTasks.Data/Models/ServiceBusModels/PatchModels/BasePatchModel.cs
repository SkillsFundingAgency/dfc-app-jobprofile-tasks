using System;

namespace DFC.App.JobProfileTasks.Data.Models.ServiceBusModels.PatchModels
{
    public class BasePatchModel
    {
        public Guid JobProfileId { get; set; }

        public Guid Id { get; set; }
    }
}