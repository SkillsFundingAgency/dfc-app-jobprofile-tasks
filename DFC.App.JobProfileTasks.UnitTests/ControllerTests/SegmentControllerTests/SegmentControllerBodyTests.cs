using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using DFC.App.JobProfileTasks.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace DFC.App.JobProfileTasks.UnitTests.ControllerTests.SegmentControllerTests
{
    public class SegmentControllerBodyTests : BaseSegmentController
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async void ReturnsSuccessForHtmlMediaType(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            var expectedResult = A.Fake<JobProfileTasksSegmentModel>();
            var controller = BuildSegmentController(mediaTypeName);

            expectedResult.CanonicalName = article;

            A.CallTo(() => FakeJobProfileSegmentService.GetByNameAsync(A<string>.Ignored, A<bool>.Ignored)).Returns(expectedResult);
            A.CallTo(() => Mapper.Map<BodyViewModel>(A<JobProfileTasksSegmentModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSegmentService.GetByNameAsync(A<string>.Ignored, A<bool>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => Mapper.Map<BodyViewModel>(A<JobProfileTasksSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BodyViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async void ReturnsSuccessForJsonMediaType(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            var expectedResult = A.Fake<JobProfileTasksSegmentModel>();
            var controller = BuildSegmentController(mediaTypeName);

            expectedResult.CanonicalName = article;

            A.CallTo(() => FakeJobProfileSegmentService.GetByNameAsync(A<string>.Ignored, A<bool>.Ignored)).Returns(expectedResult);
            A.CallTo(() => Mapper.Map<BodyViewModel>(A<JobProfileTasksSegmentModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSegmentService.GetByNameAsync(A<string>.Ignored, A<bool>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => Mapper.Map<BodyViewModel>(A<JobProfileTasksSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<BodyViewModel>(jsonResult.Value);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(InvalidMediaTypes))]
        public async void ReturnsNotAcceptableForInvalidMediaType(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            var expectedResult = A.Fake<JobProfileTasksSegmentModel>();
            var controller = BuildSegmentController(mediaTypeName);

            expectedResult.CanonicalName = article;

            A.CallTo(() => FakeJobProfileSegmentService.GetByNameAsync(A<string>.Ignored, A<bool>.Ignored)).Returns(expectedResult);
            A.CallTo(() => Mapper.Map<BodyViewModel>(A<JobProfileTasksSegmentModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSegmentService.GetByNameAsync(A<string>.Ignored, A<bool>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => Mapper.Map<BodyViewModel>(A<JobProfileTasksSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.NotAcceptable, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}
