using DFC.App.JobProfileTasks.Data.Contracts;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfileTasks.Data.Models
{
    public class JobProfileTaskSegmentModel : IDataModel
    {
        [JsonProperty(PropertyName = "id")]
        public Guid DocumentId { get; set; }

        [Required]
        [JsonProperty(PropertyName = "canonicalName")]
        public string CanonicalName { get; set; }

        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }

        [Display(Name = "Last Reviewed")]
        [JsonProperty(PropertyName = "lastReviewed")]
        public DateTime LastReviewed { get; set; }
    }
}
