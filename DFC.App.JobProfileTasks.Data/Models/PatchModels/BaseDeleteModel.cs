﻿using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfileTasks.Data.Models.PatchModels
{
    public class BaseDeleteModel
    {
        [Required]
        public Guid JobProfileId { get; set; }

        [Required]
        public Guid Id { get; set; }
    }
}