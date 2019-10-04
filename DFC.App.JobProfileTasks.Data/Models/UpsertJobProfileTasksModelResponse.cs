using System.Net;

namespace DFC.App.JobProfileTasks.Data.Models
{
    public class UpsertJobProfileTasksModelResponse
    {
        public JobProfileTasksSegmentModel JobProfileTasksSegmentModel { get; set; }

        public HttpStatusCode ResponseStatusCode { get; set; }
    }
}