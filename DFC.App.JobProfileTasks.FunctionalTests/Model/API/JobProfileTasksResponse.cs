using System;
using System.Collections.Generic;

namespace DFC.App.JobProfileTasks.FunctionalTests.Model.API
{
    public class JobProfileTasksResponse
    {
        public List<string> wydDayToDayTasks { get; set; }
        public WorkingEnvironment workingEnvironment { get; set; }
    }
}
