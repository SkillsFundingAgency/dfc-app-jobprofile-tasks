﻿namespace DFC.App.JobProfileTasks.Data.Models.PatchModels
{
    public class PatchUniformModel : BasePatchModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public bool IsNegative { get; set; }
    }
}