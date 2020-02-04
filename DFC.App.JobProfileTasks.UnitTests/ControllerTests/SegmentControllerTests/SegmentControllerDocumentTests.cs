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
    public class SegmentControllerDocumentTests : BaseSegmentController
    {
        private const string Article = "an-article-name";

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task ReturnsSuccessForHtmlMediaType(string mediaTypeName)
        {
            // Arrange
            var expectedResult = A.Fake<JobProfileTasksSegmentModel>();
            var controller = BuildSegmentController(mediaTypeName);
            var bodyViewModel = GetBodyViewModel();

            A.CallTo(() => FakeJobProfileSegmentService.GetByNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<JobProfileTasksSegmentModel>.Ignored)).Returns(bodyViewModel);

            // Act
            var result = await controller.Document(Article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSegmentService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<JobProfileTasksSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<BodyViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task ReturnsNoContentWhenNoData(string mediaTypeName)
        {
            // Arrange
            JobProfileTasksSegmentModel expectedResult = null;
            var controller = BuildSegmentController(mediaTypeName);

            A.CallTo(() => FakeJobProfileSegmentService.GetByNameAsync(A<string>.Ignored)).Returns(expectedResult);

            // Act
            var result = await controller.Document(Article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSegmentService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<NoContentResult>(result);

            Assert.Equal((int)HttpStatusCode.NoContent, statusResult.StatusCode);

            controller.Dispose();
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