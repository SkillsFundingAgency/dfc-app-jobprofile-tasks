using DFC.App.JobProfileTasks.ApiModels;
using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using DFC.App.JobProfileTasks.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileTasks.UnitTests.ControllerTests.SegmentControllerTests
{
    public class SegmentControllerBodyTests : BaseSegmentController
    {
        private const string ArticleName = "an-article-name";
        private readonly Guid documentId = Guid.NewGuid();

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task ReturnsSuccessForHtmlMediaType(string mediaTypeName)
        {
            // Arrange
            var expectedResult = A.Fake<JobProfileTasksSegmentModel>();
            var controller = BuildSegmentController(mediaTypeName);
            var viewModel = GetBodyViewModel();

            expectedResult.CanonicalName = ArticleName;

            A.CallTo(() => FakeJobProfileSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<JobProfileTasksSegmentModel>.Ignored)).Returns(viewModel);

            // Act
            var result = await controller.Body(documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSegmentService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<JobProfileTasksSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<BodyViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ReturnsSuccessForJsonMediaType(string mediaTypeName)
        {
            // Arrange
            var expectedResult = new JobProfileTasksSegmentModel();
            var controller = BuildSegmentController(mediaTypeName);
            var apiModel = GetDummyApiModel();

            expectedResult.CanonicalName = ArticleName;

            A.CallTo(() => FakeJobProfileSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map<WhatYouWillDoApiModel>(A<JobProfileTasksDataSegmentModel>.Ignored)).Returns(apiModel);

            // Act
            var result = await controller.Body(documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSegmentService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<WhatYouWillDoApiModel>(A<JobProfileTasksDataSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<WhatYouWillDoApiModel>(jsonResult.Value);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(InvalidMediaTypes))]
        public async Task ReturnsNotAcceptableForInvalidMediaType(string mediaTypeName)
        {
            // Arrange
            var expectedResult = A.Fake<JobProfileTasksSegmentModel>();
            var controller = BuildSegmentController(mediaTypeName);
            var viewModel = GetBodyViewModel();

            expectedResult.CanonicalName = ArticleName;

            A.CallTo(() => FakeJobProfileSegmentService.GetByIdAsync(A<Guid>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<JobProfileTasksSegmentModel>.Ignored)).Returns(viewModel);

            // Act
            var result = await controller.Body(documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSegmentService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<JobProfileTasksSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            Assert.Equal((int)HttpStatusCode.NotAcceptable, statusResult.StatusCode);

            controller.Dispose();
        }

        [Fact]
        public async Task ReturnsNoContentWhenDocumentDoesNotExist()
        {
            // Arrange
            var controller = BuildSegmentController();
            A.CallTo(() => FakeJobProfileSegmentService.GetByIdAsync(A<Guid>.Ignored))
                .Returns((JobProfileTasksSegmentModel)null);

            // Act
            var result = await controller.Body(documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSegmentService.GetByIdAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<WhatYouWillDoApiModel>(A<JobProfileTasksSegmentModel>.Ignored))
                .MustNotHaveHappened();

            Assert.IsType<NoContentResult>(result);

            controller.Dispose();
        }

        private static WhatYouWillDoApiModel GetDummyApiModel()
        {
            return new WhatYouWillDoApiModel
            {
                WorkingEnvironment = new WorkingEnvironmentApiModel
                {
                    Environment = "Environment1",
                    Location = "Location1",
                    Uniform = "Uniform1",
                },
                WYDDayToDayTasks = new List<string> { "test1", "test2" },
            };
        }

        private BodyViewModel GetBodyViewModel()
        {
            return new BodyViewModel
            {
                DocumentId = Guid.NewGuid(),
                CanonicalName = "job-profile-1",
                Data = new BodyDataViewModel
                {
                    Environment = "Environment1",
                    Location = "Location1",
                    Uniform = "Uniform1",
                    Introduction = "Introduction1",
                    Tasks = "Tasks",
                },
            };
        }
    }
}