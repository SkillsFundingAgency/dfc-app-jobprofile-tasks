using DFC.App.JobProfileTasks.Controllers;
using DFC.App.JobProfileTasks.Data.Enums;
using DFC.App.JobProfileTasks.Data.Models.PatchModels;
using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileTasks.UnitTests.ControllerTests.SegmentControllerTests
{
    public class SegmentControllerPatchTests : BaseSegmentController
    {
        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ReturnsSuccessForPatchEnvironment(string mediaTypeName)
        {
            // Arrange
            var patchModel = new PatchEnvironmentsModel()
            {
                JobProfileId = Guid.NewGuid(),
                Description = "Description1",
                Id = Guid.NewGuid(),
                MessageAction = MessageActionType.Published,
                SequenceNumber = 1,
            };
            var controller = BuildSegmentController(mediaTypeName);

            //Act
            var result = await controller.PatchEnvironment(patchModel).ConfigureAwait(false);

            //Assert
            A.CallTo(() => FakeJobProfileSegmentService.UpdateEnvironment(patchModel.JobProfileId, A<JobProfileTasksDataEnvironmentSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.IsType<StatusCodeResult>(result);
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ReturnsSuccessForPatchLocation(string mediaTypeName)
        {
            // Arrange
            var patchModel = new PatchLocationModel()
            {
                JobProfileId = Guid.NewGuid(),
                Description = "Description1",
                Id = Guid.NewGuid(),
                MessageAction = MessageActionType.Published,
                SequenceNumber = 1,
            };
            var controller = BuildSegmentController(mediaTypeName);

            //Act
            var result = await controller.PatchLocation(patchModel).ConfigureAwait(false);

            //Assert
            A.CallTo(() => FakeJobProfileSegmentService.UpdateLocation(patchModel.JobProfileId, A<JobProfileTasksDataLocationSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.IsType<StatusCodeResult>(result);
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ReturnsSuccessForPatchUniform(string mediaTypeName)
        {
            // Arrange
            var patchModel = new PatchUniformModel()
            {
                JobProfileId = Guid.NewGuid(),
                Description = "Description1",
                Id = Guid.NewGuid(),
                MessageAction = MessageActionType.Published,
                SequenceNumber = 1,
            };
            var controller = BuildSegmentController(mediaTypeName);

            //Act
            var result = await controller.PatchUniform(patchModel).ConfigureAwait(false);

            //Assert
            A.CallTo(() => FakeJobProfileSegmentService.UpdateUniform(patchModel.JobProfileId, A<JobProfileTasksDataUniformSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.IsType<StatusCodeResult>(result);
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ReturnsSuccessForDeleteEnvironment(string mediaTypeName)
        {
            // Arrange
            var patchModel = new DeleteEnvironmentModel()
            {
                JobProfileId = Guid.NewGuid(),
                Id = Guid.NewGuid(),
            };
            var controller = BuildSegmentController(mediaTypeName);

            //Act
            var result = await controller.DeleteEnvironment(patchModel).ConfigureAwait(false);

            //Assert
            A.CallTo(() => FakeJobProfileSegmentService.DeleteEnvironment(patchModel.JobProfileId, patchModel.Id)).MustHaveHappenedOnceExactly();
            Assert.IsType<StatusCodeResult>(result);
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ReturnsSuccessForDeleteLocation(string mediaTypeName)
        {
            // Arrange
            var patchModel = new DeleteLocationModel()
            {
                JobProfileId = Guid.NewGuid(),
                Id = Guid.NewGuid(),
            };
            var controller = BuildSegmentController(mediaTypeName);

            //Act
            var result = await controller.DeleteLocation(patchModel).ConfigureAwait(false);

            //Assert
            A.CallTo(() => FakeJobProfileSegmentService.DeleteLocation(patchModel.JobProfileId, patchModel.Id)).MustHaveHappenedOnceExactly();
            Assert.IsType<StatusCodeResult>(result);
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ReturnsSuccessForDeleteUniform(string mediaTypeName)
        {
            // Arrange
            var patchModel = new DeleteUniformModel()
            {
                JobProfileId = Guid.NewGuid(),
                Id = Guid.NewGuid(),
            };
            var controller = BuildSegmentController(mediaTypeName);

            //Act
            var result = await controller.DeleteUniform(patchModel).ConfigureAwait(false);

            //Assert
            A.CallTo(() => FakeJobProfileSegmentService.DeleteUniform(patchModel.JobProfileId, patchModel.Id)).MustHaveHappenedOnceExactly();
            Assert.IsType<StatusCodeResult>(result);
        }

        protected SegmentController BuildSegmentController(string mediaTypeName)
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.Accept] = mediaTypeName;

            var controller = new SegmentController(FakeLogger, FakeJobProfileSegmentService, Mapper)
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
