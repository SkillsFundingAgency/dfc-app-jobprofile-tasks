using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfileTasks.Data.Models.PatchModels
{
    public class DeleteUniformModel
    {
        [Required]
        public Guid JobProfileId { get; set; }

        [Required]
        public Guid UniformId { get; set; }
    }
}
