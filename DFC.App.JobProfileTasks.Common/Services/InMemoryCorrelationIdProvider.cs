using DFC.App.JobProfileTasks.Common.Contracts;

namespace DFC.App.JobProfileTasks.Common.Services
{
    public class InMemoryCorrelationIdProvider : ICorrelationIdProvider
    {
        public string CorrelationId { get; set; }
    }
}