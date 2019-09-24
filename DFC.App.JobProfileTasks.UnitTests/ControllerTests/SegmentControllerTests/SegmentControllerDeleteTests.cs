using DFC.App.JobProfileTasks.Data.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using Xunit;

namespace DFC.App.JobProfileTasks.UnitTests.ControllerTests.SegmentControllerTests
{
    public class SegmentControllerDeleteTests : BaseSegmentController
    {
        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void ReturnsSuccessWhenDocumentIdExists(string mediaTypeName)
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var expectedResult = A.Fake<JobProfileTasksSegmentModel>();
            expectedResult.DocumentId = documentId;
            var controller = BuildSegmentController(mediaTypeName);

            A.CallTo(() => JobProfileSegmentService.GetByIdAsync(documentId)).Returns(expectedResult);

            // Act
            var result = await controller.Delete(documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => JobProfileSegmentService.DeleteAsync(documentId, A<int>.Ignored)).MustHaveHappenedOnceExactly();

            var okResult = Assert.IsType<OkResult>(result);

            A.Equals((int)HttpStatusCode.OK, okResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void ReturnsNotFoundWhenDocumentIdDoesNotExist(string mediaTypeName)
        {
            // Arrange
            var documentId = Guid.NewGuid();
            JobProfileTasksSegmentModel expectedResult = null;
            var controller = BuildSegmentController(mediaTypeName);

            A.CallTo(() => JobProfileSegmentService.GetByIdAsync(documentId)).Returns(expectedResult);

            // Act
            var result = await controller.Delete(documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => JobProfileSegmentService.GetByIdAsync(documentId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => JobProfileSegmentService.DeleteAsync(documentId, A<int>.Ignored)).MustNotHaveHappened();

            var statusResult = Assert.IsType<NotFoundResult>(result);

            A.Equals((int)HttpStatusCode.NotFound, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}