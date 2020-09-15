namespace DFC.App.JobProfileTasks.FunctionalTests.Model.Support
{
    public class AppSettings
    {
        public int DeploymentWaitInMinutes { get; set; }

        public ServiceBusConfig ServiceBusConfig { get; set; } = new ServiceBusConfig();

        public APIConfig APIConfig { get; set; } = new APIConfig();
    }
}
