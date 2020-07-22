using DFC.App.JobProfileTasks.ViewModels;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileTasks.UnitTests.ControllerTests.HealthControllerTests
{
    public class HealthControllerHealthTests : BaseHealthController
    {
        [Fact]
        public async Task ReturnsSuccessWhenhealthy()
        {
            // Arrange
            var expectedResult = true;
            var controller = BuildHealthController(MediaTypeNames.Application.Json);

            A.CallTo(() => JobProfileTasksSegmentService.PingAsync()).Returns(expectedResult);

            // Act
            var result = await controller.Health().ConfigureAwait(false);

            // Assert
            A.CallTo(() => JobProfileTasksSegmentService.PingAsync()).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<HealthViewModel>(jsonResult.Value);

            jsonResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            model.HealthItems.Count.Should().BeGreaterThan(0);
            model.HealthItems.First().Service.Should().NotBeNullOrWhiteSpace();
            model.HealthItems.First().Message.Should().NotBeNullOrWhiteSpace();

            controller.Dispose();
        }

        [Fact]
        public async Task ReturnsServiceUnavailableWhenUnhealthy()
        {
            // Arrange
            var expectedResult = false;
            var controller = BuildHealthController(MediaTypeNames.Application.Json);

            A.CallTo(() => JobProfileTasksSegmentService.PingAsync()).Returns(expectedResult);

            // Act
            var result = await controller.Health().ConfigureAwait(false);

            // Assert
            A.CallTo(() => JobProfileTasksSegmentService.PingAsync()).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            statusResult.StatusCode.Should().Be((int)HttpStatusCode.ServiceUnavailable);

            controller.Dispose();
        }

        [Fact]
        public async Task ReturnsServiceUnavailableWhenException()
        {
            // Arrange
            var controller = BuildHealthController(MediaTypeNames.Application.Json);

            A.CallTo(() => JobProfileTasksSegmentService.PingAsync()).Throws<Exception>();

            // Act
            var result = await controller.Health().ConfigureAwait(false);

            // Assert
            A.CallTo(() => JobProfileTasksSegmentService.PingAsync()).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            statusResult.StatusCode.Should().Be((int)HttpStatusCode.ServiceUnavailable);

            controller.Dispose();
        }
    }
}