using System;

namespace DFC.App.JobProfileTasks.Data.Models
{
    public class JobProfileTasksDataLocationSegmentModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsNegative { get; set; }
    }
}
