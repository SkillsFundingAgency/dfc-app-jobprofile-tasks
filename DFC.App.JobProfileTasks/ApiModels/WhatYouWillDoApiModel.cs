using System.Collections.Generic;

namespace DFC.App.JobProfileTasks.ApiModels
{
    public class WhatYouWillDoApiModel
    {
        public List<string> WYDDayToDayTasks { get; set; }

        public WorkingEnvironmentApiModel WorkingEnvironment { get; set; }
    }
}