using DFC.App.JobProfileTasks.Common.Constants;
using DFC.App.JobProfileTasks.Common.Contracts;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using System.Collections.Generic;

namespace DFC.App.JobProfileTasks.Common.Services
{
    public class LogService : ILogService
    {
        private readonly ICorrelationIdProvider correlationIdProvider;
        private readonly TelemetryClient telemetryClient;

        public LogService(ICorrelationIdProvider correlationIdProvider, TelemetryClient telemetryClient)
        {
            this.correlationIdProvider = correlationIdProvider;
            this.telemetryClient = telemetryClient;
        }

        public void LogMessage(string message, SeverityLevel severityLevel = SeverityLevel.Information)
        {
            Log(message, severityLevel);
        }

        private void Log(string message, SeverityLevel severityLevel)
        {
            var properties = new Dictionary<string, string>
            {
                {HeaderName.CorrelationId, correlationIdProvider.CorrelationId}
            };
            telemetryClient.TrackTrace(message, severityLevel, properties);
        }
    }
}