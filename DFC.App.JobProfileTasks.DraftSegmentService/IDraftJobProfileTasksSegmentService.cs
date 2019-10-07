﻿using DFC.App.JobProfileTasks.Data.Models;
using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.DraftSegmentService
{
    public interface IDraftJobProfileTasksSegmentService
    {
        Task<JobProfileTasksSegmentModel> GetSitefinityData(string canonicalName);
    }
}