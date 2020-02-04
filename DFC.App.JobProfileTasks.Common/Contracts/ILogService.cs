using Microsoft.ApplicationInsights.DataContracts;

namespace DFC.App.JobProfileTasks.Common.Contracts
{
    public interface ILogService
    {
        void LogMessage(string message, SeverityLevel severityLevel = SeverityLevel.Information);
    }
}