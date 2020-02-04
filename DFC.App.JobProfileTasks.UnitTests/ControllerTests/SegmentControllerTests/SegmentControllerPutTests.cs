using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileTasks.UnitTests.ControllerTests.SegmentControllerTests
{
    public class SegmentControllerPutTests : BaseSegmentController
    {
        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            var existingModel = new JobProfileTasksSegmentModel { SequenceNumber = 123 };
            var modelToUpsert = new JobProfileTasksSegmentModel { SequenceNumber = 124 };
            var controller = BuildSegmentController(mediaTypeName);
            var expectedUpsertResponse = HttpStatusCode.OK;

            A.CallTo(() => FakeJobProfileSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(existingModel);
            A.CallTo(() => FakeJobProfileSegmentService.UpsertAsync(A<JobProfileTasksSegmentModel>.Ignored)).Returns(expectedUpsertResponse);

            // Act
            var result = await controller.Put(modelToUpsert).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSegmentService.UpsertAsync(A<JobProfileTasksSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, statusCodeResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ReturnsBadResultWhenModelIsNull(string mediaTypeName)
        {
            // Arrange
            var controller = BuildSegmentController(mediaTypeName);

            // Act
            var result = await controller.Put(null).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ReturnsBadResultWhenModelIsInvalid(string mediaTypeName)
        {
            // Arrange
            var jobProfileTasksSegmentModel = new JobProfileTasksSegmentModel();
            var controller = BuildSegmentController(mediaTypeName);

            controller.ModelState.AddModelError(string.Empty, "Model is not valid");

            // Act
            var result = await controller.Put(jobProfileTasksSegmentModel).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }

        [Fact]
        public async Task ReturnsNotFoundWhenDocumentDoesNotAlreadyExist()
        {
            // Arrange
            var jobProfileTasksSegmentModel = new JobProfileTasksSegmentModel();
            var controller = BuildSegmentController();

            A.CallTo(() => FakeJobProfileSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns((JobProfileTasksSegmentModel)null);

            // Act
            var result = await controller.Put(jobProfileTasksSegmentModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSegmentService.UpsertAsync(A<JobProfileTasksSegmentModel>.Ignored)).MustNotHaveHappened();
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);

            Assert.Equal((int)HttpStatusCode.NotFound, statusCodeResult.StatusCode);

            controller.Dispose();
        }

        [Fact]
        public async Task ReturnsAlreadyReportedWhenExistingSequenceNumberIsHigher()
        {
            // Arrange
            var existingModel = new JobProfileTasksSegmentModel { SequenceNumber = 999 };
            var modelToUpsert = new JobProfileTasksSegmentModel { SequenceNumber = 124 };
            var controller = BuildSegmentController();

            A.CallTo(() => FakeJobProfileSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(existingModel);

            // Act
            var result = await controller.Put(modelToUpsert).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSegmentService.UpsertAsync(A<JobProfileTasksSegmentModel>.Ignored)).MustNotHaveHappened();
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.AlreadyReported, statusCodeResult.StatusCode);

            controller.Dispose();
        }
    }
}