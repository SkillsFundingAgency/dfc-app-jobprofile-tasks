using DFC.App.JobProfileTasks.ApiModels;
using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using DFC.App.JobProfileTasks.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileTasks.UnitTests.ControllerTests.SegmentControllerTests
{
    public class SegmentControllerBodyTests : BaseSegmentController
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task ReturnsSuccessForHtmlMediaType(string mediaTypeName)
        {
            // Arrange
            var article = Guid.NewGuid();
            var expectedResult = A.Fake<JobProfileTasksSegmentModel>();
            var controller = BuildSegmentController(mediaTypeName);

            expectedResult.CanonicalName = article.ToString();

            A.CallTo(() => FakeJobProfileSegmentService.GetByIdAsync(article)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<JobProfileTasksSegmentModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSegmentService.GetByIdAsync(article)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<JobProfileTasksSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BodyViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ReturnsSuccessForJsonMediaType(string mediaTypeName)
        {
            // Arrange
            var article = Guid.NewGuid();
            var expectedResult = new JobProfileTasksSegmentModel();
            var controller = BuildSegmentController(mediaTypeName);

            expectedResult.CanonicalName = article.ToString();

            A.CallTo(() => FakeJobProfileSegmentService.GetByIdAsync(article)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map<WhatYouWillDoApiModel>(A<JobProfileTasksDataSegmentModel>.Ignored)).Returns(A.Fake<WhatYouWillDoApiModel>());

            // Act
            var result = await controller.Body(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSegmentService.GetByIdAsync(article)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<WhatYouWillDoApiModel>(A<JobProfileTasksDataSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<WhatYouWillDoApiModel>(jsonResult.Value);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(InvalidMediaTypes))]
        public async Task ReturnsNotAcceptableForInvalidMediaType(string mediaTypeName)
        {
            // Arrange
            var article = Guid.NewGuid();
            var expectedResult = A.Fake<JobProfileTasksSegmentModel>();
            var controller = BuildSegmentController(mediaTypeName);

            expectedResult.CanonicalName = article.ToString();

            A.CallTo(() => FakeJobProfileSegmentService.GetByIdAsync(article)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<JobProfileTasksSegmentModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSegmentService.GetByIdAsync(article)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<JobProfileTasksSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.NotAcceptable, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}