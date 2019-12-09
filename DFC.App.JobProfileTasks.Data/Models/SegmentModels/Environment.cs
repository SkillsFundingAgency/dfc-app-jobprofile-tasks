using System;

namespace DFC.App.JobProfileTasks.Data.Models.SegmentModels
{
    public class Environment
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public bool IsNegative { get; set; }
    }
}