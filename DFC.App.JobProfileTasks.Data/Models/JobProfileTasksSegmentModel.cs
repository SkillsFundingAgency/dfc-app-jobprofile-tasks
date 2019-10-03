using DFC.App.JobProfileTasks.Data.Contracts;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfileTasks.Data.Models
{
    public class JobProfileTasksSegmentModel : IDataModel
    {
        [JsonProperty(PropertyName = "id")]
        public Guid DocumentId { get; set; }

        [JsonProperty(PropertyName = "_etag")]
        public string Etag { get; set; }

        [Required]
        public string CanonicalName { get; set; }

        public DateTime LastReviewed { get; set; }

        public string PartitionKey => SocLevelTwo;

        [Required]
        public string SocLevelTwo { get; set; }

        public JobProfileTasksDataSegmentModel Data { get; set; }
    }
}