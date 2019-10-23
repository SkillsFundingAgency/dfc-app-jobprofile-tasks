using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfileTasks.ViewModels
{
    public class BodyViewModel
    {
        [Display(Name = "Document Id")]
        public Guid? DocumentId { get; set; }

        [Display(Name = "Canonical Name")]
        public string CanonicalName { get; set; }

        public DateTime LastModified { get; set; }

        public BodyDataViewModel Data { get; set; }
    }
}