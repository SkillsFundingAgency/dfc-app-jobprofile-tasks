using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using DFC.App.JobProfileTasks.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Net.Mime;
using Xunit;

namespace DFC.App.JobProfileTasks.UnitTests.ControllerTests.SegmentControllerTests
{
    public class SegmentControllerPatchTests : BaseSegmentController
    {
        //[Fact]
        //public async void Returns404WhenNothingToPatch()
        //{
        //    // Arrange
        //    var documentId = Guid.NewGuid();
        //    var mediaTypeName = MediaTypeNames.Application.Json;
        //    JsonPatchDocument<JobProfileTasksSegmentModel> patchDocument = null;
        //    var persistedDocument = A.Fake<JobProfileTasksSegmentModel>();
        //    var controller = BuildSegmentController(mediaTypeName);

        //    A.CallTo(() => FakeJobProfileSegmentService.GetByIdAsync(documentId)).Returns(persistedDocument);

        //    // Act
        //    var result = await controller.PatchUniform(patchDocument, documentId).ConfigureAwait(false);

        //    // Assert
        //    A.CallTo(() => FakeJobProfileSegmentService.GetByNameAsync(A<string>.Ignored, A<bool>.Ignored)).MustNotHaveHappened();
        //    A.CallTo(() => Mapper.Map<DocumentViewModel>(A<JobProfileTasksSegmentModel>.Ignored)).MustNotHaveHappened();

        //    var badRequestResult = Assert.IsType<BadRequestResult>(result);
        //    Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);

        //    controller.Dispose();
        //}

        //[Fact]
        //public async void ReturnsModelWhenPatching()
        //{
        //    // Arrange
        //    var documentId = Guid.NewGuid();
        //    var mediaTypeName = MediaTypeNames.Application.Json;
        //    var patchDocument = A.Fake<JsonPatchDocument<JobProfileTasksSegmentModel>>();
        //    var persistedDocument = A.Fake<JobProfileTasksSegmentModel>();
        //    var controller = BuildSegmentController(mediaTypeName);

        //    A.CallTo(() => FakeJobProfileSegmentService.GetByIdAsync(documentId)).Returns(persistedDocument);

        //    // Act
        //    var result = await controller.PatchUniform(patchDocument, documentId).ConfigureAwait(false);

        //    // Assert
        //    A.CallTo(() => FakeJobProfileSegmentService.GetByIdAsync(documentId)).MustHaveHappenedOnceExactly();
        //    A.CallTo(() => Mapper.Map<BodyViewModel>(A<JobProfileTasksSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();

        //    var okObjectResult = Assert.IsType<OkObjectResult>(result);

        //    controller.Dispose();
        //}
    }
}