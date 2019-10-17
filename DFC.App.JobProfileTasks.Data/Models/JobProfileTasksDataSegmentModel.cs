using System.Collections.Generic;

namespace DFC.App.JobProfileTasks.Data.Models
{
    public class JobProfileTasksDataSegmentModel
    {
        public string Introduction { get; set; }

        public string Tasks { get; set; }

        public IEnumerable<JobProfileTasksDataLocationSegmentModel> Location { get; set; }

        public IEnumerable<JobProfileTasksDataEnvironmentSegmentModel> Environment { get; set; }

        public IEnumerable<JobProfileTasksDataUniformSegmentModel> Uniform { get; set; }
    }
}