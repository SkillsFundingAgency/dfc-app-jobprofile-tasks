using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileTasks.UnitTests.ControllerTests.SegmentControllerTests
{
    public class SegmentControllerPostTests : BaseSegmentController
    {
        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            var tasksSegmentModel = A.Fake<JobProfileTasksSegmentModel>();
            var controller = BuildSegmentController(mediaTypeName);
            var expectedUpsertResponse = BuildExpectedUpsertResponse(A.Fake<JobProfileTasksSegmentModel>(), HttpStatusCode.AlreadyReported);

            A.CallTo(() => FakeJobProfileSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns<JobProfileTasksSegmentModel>(null);
            A.CallTo(() => FakeJobProfileSegmentService.UpsertAsync(A<JobProfileTasksSegmentModel>.Ignored)).Returns(expectedUpsertResponse);

            // Act
            var result = await controller.Post(tasksSegmentModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSegmentService.UpsertAsync(A<JobProfileTasksSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);

            Assert.Equal((int)HttpStatusCode.AlreadyReported, statusCodeResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ReturnsAlreadyReportedIfEntityExists(string mediaTypeName)
        {
            // Arrange
            var tasksSegmentModel = A.Fake<JobProfileTasksSegmentModel>();
            var controller = BuildSegmentController(mediaTypeName);
            var expectedUpsertResponse = BuildExpectedUpsertResponse(A.Fake<JobProfileTasksSegmentModel>());

            A.CallTo(() => FakeJobProfileSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(tasksSegmentModel);
            A.CallTo(() => FakeJobProfileSegmentService.UpsertAsync(A<JobProfileTasksSegmentModel>.Ignored)).Returns(expectedUpsertResponse);

            // Act
            var result = await controller.Post(tasksSegmentModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSegmentService.UpsertAsync(A<JobProfileTasksSegmentModel>.Ignored)).MustNotHaveHappened();
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);

            Assert.Equal((int)HttpStatusCode.AlreadyReported, statusCodeResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ReturnsBadResultWhenModelIsNull(string mediaTypeName)
        {
            // Arrange
            var controller = BuildSegmentController(mediaTypeName);

            // Act
            var result = await controller.Post(null).ConfigureAwait(false);

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
            var result = await controller.Post(jobProfileTasksSegmentModel).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }

        private UpsertJobProfileTasksModelResponse BuildExpectedUpsertResponse(JobProfileTasksSegmentModel model, HttpStatusCode statusCode = HttpStatusCode.Created)
        {
            return new UpsertJobProfileTasksModelResponse
            {
                JobProfileTasksSegmentModel = model,
                ResponseStatusCode = statusCode,
            };
        }
    }
}