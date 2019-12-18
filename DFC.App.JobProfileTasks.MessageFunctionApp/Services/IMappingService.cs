using DFC.App.JobProfileTasks.Data.Models.SegmentModels;

namespace DFC.App.JobProfileTasks.MessageFunctionApp.Services
{
    public interface IMappingService
    {
        JobProfileTasksSegmentModel MapToSegmentModel(string message, long sequenceNumber);
    }
}