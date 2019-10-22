using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfileTasks.Data.Models.PatchModels
{
    public class BasePatchItemModel : BasePatchModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Url { get; set; }

        [Required]
        public bool IsNegative { get; set; }
    }
}
