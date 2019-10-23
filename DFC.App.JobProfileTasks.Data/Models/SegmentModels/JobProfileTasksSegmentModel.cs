using DFC.App.JobProfileTasks.Data.Contracts;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfileTasks.Data.Models.SegmentModels
{
    public class JobProfileTasksSegmentModel : IDataModel
    {
        [JsonProperty(PropertyName = "id")]
        public Guid DocumentId { get; set; }

        [JsonProperty(PropertyName = "_etag")]
        public string Etag { get; set; }

        [Required]
        public string CanonicalName { get; set; }

        public string PartitionKey => SocLevelTwo;

        public long SequenceNumber { get; set; }

        [Required]
        public string SocLevelTwo { get; set; }

        public DateTime LastModified { get; set; }

        public JobProfileTasksDataSegmentModel Data { get; set; }
    }
}