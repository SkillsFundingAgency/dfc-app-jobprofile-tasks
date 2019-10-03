using DFC.App.JobProfileTasks.Data.Models;
using DFC.App.JobProfileTasks.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace DFC.App.JobProfileTasks.UnitTests.ControllerTests.SegmentControllerTests
{
    public class SegmentControllerDocumentTests : BaseSegmentController
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async void ReturnsSuccessForHtmlMediaType(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            var expectedResult = A.Fake<JobProfileTasksSegmentModel>();
            var controller = BuildSegmentController(mediaTypeName);

            A.CallTo(() => FakeJobProfileSegmentService.GetByNameAsync(A<string>.Ignored, A<bool>.Ignored)).Returns(expectedResult);
            A.CallTo(() => Mapper.Map<DocumentViewModel>(A<JobProfileTasksSegmentModel>.Ignored)).Returns(A.Fake<DocumentViewModel>());

            // Act
            var result = await controller.Document(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSegmentService.GetByNameAsync(A<string>.Ignored, A<bool>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => Mapper.Map<DocumentViewModel>(A<JobProfileTasksSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<DocumentViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async void ReturnsNoContentWhenNoData(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            JobProfileTasksSegmentModel expectedResult = null;
            var controller = BuildSegmentController(mediaTypeName);

            A.CallTo(() => FakeJobProfileSegmentService.GetByNameAsync(A<string>.Ignored, A<bool>.Ignored)).Returns(expectedResult);

            // Act
            var result = await controller.Document(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSegmentService.GetByNameAsync(A<string>.Ignored, A<bool>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<NoContentResult>(result);

            A.Equals((int)HttpStatusCode.NoContent, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}