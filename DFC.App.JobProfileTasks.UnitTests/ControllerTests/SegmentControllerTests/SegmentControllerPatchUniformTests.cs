using DFC.App.JobProfileTasks.Data.Models.PatchModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileTasks.UnitTests.ControllerTests.SegmentControllerTests
{
    public class SegmentControllerPatchUniformTests : BaseSegmentController
    {
        public static IEnumerable<object[]> ValidStatusCodes => new List<object[]>
        {
            new object[] { HttpStatusCode.OK },
            new object[] { HttpStatusCode.Created },
        };

        public static IEnumerable<object[]> InvalidStatusCodes => new List<object[]>
        {
            new object[] { HttpStatusCode.BadRequest },
            new object[] { HttpStatusCode.AlreadyReported },
            new object[] { HttpStatusCode.NotFound },
        };

        [Fact]
        public async Task SegmentControllerPatchUniformReturnsBadRequestWhenModelIsNull()
        {
            // Arrange
            var controller = BuildSegmentController();

            // Act
            var result = await controller.PatchUniform(null, Guid.NewGuid()).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }

        [Fact]
        public async Task SegmentControllerPatchUniformReturnsBadRequestWhenModelIsInvalid()
        {
            // Arrange
            var model = new PatchUniformModel();
            var controller = BuildSegmentController();
            controller.ModelState.AddModelError(string.Empty, "Model is not valid");

            // Act
            var result = await controller.PatchUniform(model, Guid.NewGuid()).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(InvalidStatusCodes))]
        public async Task SegmentControllerPatchUniformReturnsResponseAndLogsErrorWhenInvalidStatusReturned(HttpStatusCode expectedStatus)
        {
            // Arrange
            var controller = BuildSegmentController();
            var model = new PatchUniformModel();

            A.CallTo(() => FakeJobProfileSegmentService.PatchUniformAsync(A<PatchUniformModel>.Ignored, A<Guid>.Ignored)).Returns(expectedStatus);

            // Act
            var result = await controller.PatchUniform(model, Guid.NewGuid()).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSegmentService.PatchUniformAsync(A<PatchUniformModel>.Ignored, A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)expectedStatus, statusCodeResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(ValidStatusCodes))]
        public async Task SegmentControllerPatchUniformReturnsSuccessForSuccessfulResponse(HttpStatusCode expectedStatus)
        {
            // Arrange
            var controller = BuildSegmentController();
            var model = new PatchUniformModel();

            A.CallTo(() => FakeJobProfileSegmentService.PatchUniformAsync(A<PatchUniformModel>.Ignored, A<Guid>.Ignored)).Returns(expectedStatus);

            // Act
            var result = await controller.PatchUniform(model, Guid.NewGuid()).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSegmentService.PatchUniformAsync(A<PatchUniformModel>.Ignored, A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)expectedStatus, statusCodeResult.StatusCode);

            controller.Dispose();
        }
    }
}