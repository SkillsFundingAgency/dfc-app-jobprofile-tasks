using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace DFC.App.JobProfileTasks.UnitTests.ControllerTests.SegmentControllerTests
{
    public class SegmentControllerPostTests : BaseSegmentController
    {
        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void ReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            var tasksSegmentModel = A.Fake<JobProfileTasksSegmentModel>();
            var controller = BuildSegmentController(mediaTypeName);
            var expectedUpsertResponse = BuildExpectedUpsertResponse(A.Fake<JobProfileTasksSegmentModel>());

            A.CallTo(() => FakeJobProfileSegmentService.UpsertAsync(A<JobProfileTasksSegmentModel>.Ignored)).Returns(expectedUpsertResponse);

            // Act
            var result = await controller.Post(tasksSegmentModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSegmentService.UpsertAsync(A<JobProfileTasksSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            var okResult = Assert.IsType<CreatedAtActionResult>(result);

            Assert.Equal((int)HttpStatusCode.Created, okResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void ReturnsBadResultWhenModelIsNull(string mediaTypeName)
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
        public async void ReturnsBadResultWhenModelIsInvalid(string mediaTypeName)
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

        private UpsertJobProfileTasksModelResponse BuildExpectedUpsertResponse(JobProfileTasksSegmentModel model)
        {
            return new UpsertJobProfileTasksModelResponse
            {
                JobProfileTasksSegmentModel = model,
            };
        }
    }
}