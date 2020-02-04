using System;
using System.Collections.Generic;

namespace DFC.App.JobProfileTasks.Data.Models.SegmentModels
{
    public class JobProfileTasksDataSegmentModel
    {
        public const string SegmentName = "WhatYouWillDo";

        public DateTime LastReviewed { get; set; }

        public string Introduction { get; set; }

        public string Tasks { get; set; }

        public IList<Location> Locations { get; set; }

        public IList<Uniform> Uniforms { get; set; }

        public IList<Environment> Environments { get; set; }
    }
}