using Microsoft.AspNetCore.Html;
using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfileTasks.ViewModels
{
    public class DocumentViewModel
    {
        [Display(Name = "Document Id")]
        public Guid? DocumentId { get; set; }

        [Display(Name = "Canonical Name")]
        public string CanonicalName { get; set; }
    }
}
