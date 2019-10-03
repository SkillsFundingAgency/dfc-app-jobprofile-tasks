using DFC.App.JobProfileTasks.Controllers;
using DFC.App.JobProfileTasks.SegmentService;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace DFC.App.JobProfileTasks.UnitTests.ControllerTests.HealthControllerTests
{
    public abstract class BaseHealthController
    {
        public BaseHealthController()
        {
            Logger = A.Fake<ILogger<HealthController>>();
            JobProfileTasksSegmentService = A.Fake<IJobProfileTasksSegmentService>();
        }

        protected ILogger<HealthController> Logger { get; }

        protected IJobProfileTasksSegmentService JobProfileTasksSegmentService { get; }

        protected HealthController BuildHealthController(string mediaTypeName)
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.Accept] = mediaTypeName;

            var controller = new HealthController(Logger, JobProfileTasksSegmentService)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                },
            };

            return controller;
        }
    }
}