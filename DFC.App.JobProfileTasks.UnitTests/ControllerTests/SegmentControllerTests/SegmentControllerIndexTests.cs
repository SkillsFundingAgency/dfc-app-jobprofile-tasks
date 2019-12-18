using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using DFC.App.JobProfileTasks.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileTasks.UnitTests.ControllerTests.SegmentControllerTests
{
    public class SegmentControllerIndexTests : BaseSegmentController
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task ReturnsSuccessForHtmlMediaType(string mediaTypeName)
        {
            // Arrange
            const int resultsCount = 2;
            var expectedResults = A.CollectionOfFake<JobProfileTasksSegmentModel>(resultsCount);
            var controller = BuildSegmentController(mediaTypeName);
            var indexDocumentViewModel = new IndexDocumentViewModel { CanonicalName = "job-profile-1" };

            A.CallTo(() => FakeJobProfileSegmentService.GetAllAsync()).Returns(expectedResults);
            A.CallTo(() => FakeMapper.Map<IndexDocumentViewModel>(A<JobProfileTasksSegmentModel>.Ignored)).Returns(indexDocumentViewModel);

            // Act
            var result = await controller.Index().ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSegmentService.GetAllAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<IndexDocumentViewModel>(A<JobProfileTasksSegmentModel>.Ignored)).MustHaveHappened(resultsCount, Times.Exactly);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IndexViewModel>(viewResult.ViewData.Model);

            Assert.Equal(model.Documents.Count(), resultsCount);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task ReturnsSuccessWhenNoData(string mediaTypeName)
        {
            // Arrange
            const int resultsCount = 0;
            IEnumerable<JobProfileTasksSegmentModel> expectedResults = null;
            var controller = BuildSegmentController(mediaTypeName);
            var indexDocumentViewModel = new IndexDocumentViewModel { CanonicalName = "job-profile-1" };

            A.CallTo(() => FakeJobProfileSegmentService.GetAllAsync()).Returns(expectedResults);
            A.CallTo(() => FakeMapper.Map<IndexDocumentViewModel>(A<JobProfileTasksSegmentModel>.Ignored)).Returns(indexDocumentViewModel);

            // Act
            var result = await controller.Index().ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileSegmentService.GetAllAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<IndexDocumentViewModel>(A<JobProfileTasksSegmentModel>.Ignored)).MustHaveHappened(resultsCount, Times.Exactly);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IndexViewModel>(viewResult.ViewData.Model);

            Assert.Null(model.Documents);

            controller.Dispose();
        }
    }
}