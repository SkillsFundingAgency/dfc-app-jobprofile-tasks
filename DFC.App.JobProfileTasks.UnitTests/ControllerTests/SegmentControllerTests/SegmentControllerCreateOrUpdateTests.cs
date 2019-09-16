using DFC.App.JobProfileTasks.Data.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using Xunit;

namespace DFC.App.JobProfileTasks.UnitTests.ControllerTests.SegmentControllerTests
{
    public class SegmentControllerCreateOrUpdateTests : BaseSegmentController
    {
        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void CreateOrUpdateReturnsSuccessForCreate(string mediaTypeName)
        {
            // Arrange
            var tasksSegmentModel = A.Fake<JobProfileTasksSegmentModel>();
            JobProfileTasksSegmentModel existingJobProfileTasksSegmentModel = null;
            var createdJobProfileTasksSegmentModel = A.Fake<JobProfileTasksSegmentModel>();
            var controller = BuildSegmentController(mediaTypeName);

            A.CallTo(() => FakeJobProfileSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(existingJobProfileTasksSegmentModel);
            A.CallTo(() => FakeJobProfileSegmentService.CreateAsync(A<JobProfileTasksSegmentModel>.Ignored)).Returns(createdJobProfileTasksSegmentModel);

            // Act
            var result = await controller.CreateOrUpdate(tasksSegmentModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSegmentService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeJobProfileSegmentService.CreateAsync(A<JobProfileTasksSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();

            var okResult = Assert.IsType<CreatedAtActionResult>(result);

            A.Equals((int)HttpStatusCode.Created, okResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void CreateOrUpdateReturnsSuccessForUpdate(string mediaTypeName)
        {
            // Arrange
            var jobProfileTasksSegmentModel = A.Fake<JobProfileTasksSegmentModel>();
            var existingJobProfileTasksSegmentModel = A.Fake<JobProfileTasksSegmentModel>();
            JobProfileTasksSegmentModel updatedJobTasksSegmentModel = null;
            var controller = BuildSegmentController(mediaTypeName);

            A.CallTo(() => FakeJobProfileSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(existingJobProfileTasksSegmentModel);
            A.CallTo(() => FakeJobProfileSegmentService.ReplaceAsync(A<JobProfileTasksSegmentModel>.Ignored)).Returns(updatedJobTasksSegmentModel);

            // Act
            var result = await controller.CreateOrUpdate(jobProfileTasksSegmentModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSegmentService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeJobProfileSegmentService.ReplaceAsync(A<JobProfileTasksSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();

            var okResult = Assert.IsType<OkObjectResult>(result);

            A.Equals((int)HttpStatusCode.OK, okResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void CreateOrUpdateReturnsBadResultWhenModelIsNull(string mediaTypeName)
        {
            // Arrange
            JobProfileTasksSegmentModel jobProfileTasksSegmentModel = null;
            var controller = BuildSegmentController(mediaTypeName);

            // Act
            var result = await controller.CreateOrUpdate(jobProfileTasksSegmentModel).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestResult>(result);

            A.Equals((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void CreateOrUpdateReturnsBadResultWhenModelIsInvalid(string mediaTypeName)
        {
            // Arrange
            var jobProfileTasksSegmentModel = new JobProfileTasksSegmentModel();
            var controller = BuildSegmentController(mediaTypeName);

            controller.ModelState.AddModelError(string.Empty, "Model is not valid");

            // Act
            var result = await controller.CreateOrUpdate(jobProfileTasksSegmentModel).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestObjectResult>(result);

            A.Equals((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}
