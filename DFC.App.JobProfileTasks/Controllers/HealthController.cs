using DFC.App.JobProfileTasks.Extensions;
using DFC.App.JobProfileTasks.SegmentService;
using DFC.App.JobProfileTasks.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.Controllers
{
    public class HealthController : Controller
    {
        private readonly ILogger<HealthController> logger;
        private readonly IJobProfileTasksSegmentService jobProfileTasksSegmentService;

        public HealthController(ILogger<HealthController> logger, IJobProfileTasksSegmentService jobProfileTasksSegmentService)
        {
            this.logger = logger;
            this.jobProfileTasksSegmentService = jobProfileTasksSegmentService;
        }

        [HttpGet]
        [Route("health")]
        public async Task<IActionResult> Health()
        {
            var resourceName = typeof(Program).Namespace;
            string message;

            logger.LogInformation($"{nameof(Health)} has been called");

            try
            {
                var isHealthy = await jobProfileTasksSegmentService.PingAsync().ConfigureAwait(false);

                if (isHealthy)
                {
                    message = "Document store is available";
                    logger.LogInformation($"{nameof(Health)} responded with: {resourceName} - {message}");

                    var viewModel = CreateHealthViewModel(resourceName, message);

                    return this.NegotiateContentResult(viewModel);
                }

                message = $"Ping to {resourceName} has failed";
                logger.LogError($"{nameof(Health)}: {message}");
            }
            catch (Exception ex)
            {
                message = $"{resourceName} exception: {ex.Message}";
                logger.LogError(ex, $"{nameof(Health)}: {message}");
            }

            return StatusCode((int)HttpStatusCode.ServiceUnavailable);
        }

        [HttpGet]
        public IActionResult Ping()
        {
            logger.LogInformation($"{nameof(Ping)} has been called");

            return Ok();
        }

        private static HealthViewModel CreateHealthViewModel(string resourceName, string message)
        {
            return new HealthViewModel
            {
                HealthItems = new List<HealthItemViewModel>
                {
                    new HealthItemViewModel
                    {
                        Service = resourceName,
                        Message = message,
                    },
                },
            };
        }
    }
}