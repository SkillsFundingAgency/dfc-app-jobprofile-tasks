using System;
using System.Collections.Generic;

namespace DFC.App.JobProfileTasks.Data.Models
{
    public class JobProfileTasksDataSegmentModel
    {
        public JobProfileTasksDataSegmentModel()
        {
            Locations = new List<JobProfileTasksDataLocationSegmentModel>();
            Environments = new List<JobProfileTasksDataEnvironmentSegmentModel>();
            Uniforms = new List<JobProfileTasksDataUniformSegmentModel>();
        }

        public DateTime LastReviewed { get; set; }

        public string Introduction { get; set; }

        public string Tasks { get; set; }

        public IEnumerable<JobProfileTasksDataLocationSegmentModel> Locations { get; set; }

        public IEnumerable<JobProfileTasksDataEnvironmentSegmentModel> Environments { get; set; }

        public IEnumerable<JobProfileTasksDataUniformSegmentModel> Uniforms { get; set; }
    }
}
