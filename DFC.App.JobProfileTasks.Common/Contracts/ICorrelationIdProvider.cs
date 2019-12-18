namespace DFC.App.JobProfileTasks.Common.Contracts
{
    public interface ICorrelationIdProvider
    {
        string CorrelationId { get; set; }
    }
}