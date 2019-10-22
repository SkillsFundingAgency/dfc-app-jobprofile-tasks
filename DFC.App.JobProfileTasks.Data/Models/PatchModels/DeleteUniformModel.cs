using System;

namespace DFC.App.JobProfileTasks.Data.Models.PatchModels
{
    public class DeleteUniformModel
    {
        public Guid JobProfileId { get; set; }

        public Guid UniformId { get; set; }
    }
}
