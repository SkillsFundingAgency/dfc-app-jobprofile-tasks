using System.Collections.Generic;

namespace DFC.App.JobProfileTasks.Data.Models.SegmentModels
{
    public class JobProfileTasksDataSegmentModel
    {
        public bool IsCadReady { get; set; }

        public string Introduction { get; set; }

        public string DailyTasks { get; set; }

        public IEnumerable<JobProfileTasksDataLocationSegmentModel> Locations { get; set; }

        public IEnumerable<JobProfileTasksDataUniformSegmentModel> Uniforms { get; set; }

        public IEnumerable<JobProfileTasksDataEnvironmentSegmentModel> Environments { get; set; }
    }
}