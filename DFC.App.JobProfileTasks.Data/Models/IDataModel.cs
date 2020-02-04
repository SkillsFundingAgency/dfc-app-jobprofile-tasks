using Newtonsoft.Json;
using System;

namespace DFC.App.JobProfileTasks.Data.Models
{
    public interface IDataModel
    {
        [JsonProperty(PropertyName = "id")]
        Guid DocumentId { get; set; }

        [JsonProperty(PropertyName = "_etag")]
        string Etag { get; set; }

        string PartitionKey { get; }
    }
}