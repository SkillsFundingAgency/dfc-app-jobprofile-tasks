﻿using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfileTasks.ViewModels
{
    public class DocumentDataViewModel
    {
        [Display(Name = "Last Updated")]
        public DateTime LastReviewed { get; set; }

        public string Introduction { get; set; }

        public string Tasks { get; set; }

        public string Location { get; set; }

        public string Environment { get; set; }

        public string Uniform { get; set; }
    }
}