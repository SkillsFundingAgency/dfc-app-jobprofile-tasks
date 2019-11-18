using DFC.App.JobProfileTasks.Data.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfileTasks.Data.Models.PatchModels
{
    public class BasePatchModel
    {
        [Required]
        public Guid JobProfileId { get; set; }

        [Required]
        public MessageActionType MessageAction { get; set; }

        [Required]
        public long SequenceNumber { get; set; }
    }
}
