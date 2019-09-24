using DFC.App.JobProfileTasks.Data.Models;
using DFC.App.JobProfileTasks.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DFC.App.JobProfileTasks.UnitTests.ControllerTests.SegmentControllerTests
{
    public class SegmentControllerIndexTests : BaseSegmentController
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async void ReturnsSuccessForHtmlMediaType(string mediaTypeName)
        {
            // Arrange
            const int resultsCount = 2;
            var expectedResults = A.CollectionOfFake<JobProfileTasksSegmentModel>(resultsCount);
            var controller = BuildSegmentController(mediaTypeName);

            A.CallTo(() => JobProfileSegmentService.GetAllAsync()).Returns(expectedResults);
            A.CallTo(() => Mapper.Map<IndexDocumentViewModel>(A<JobProfileTasksSegmentModel>.Ignored)).Returns(A.Fake<IndexDocumentViewModel>());

            // Act
            var result = await controller.Index().ConfigureAwait(false);

            // Assert
            A.CallTo(() => JobProfileSegmentService.GetAllAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => Mapper.Map<IndexDocumentViewModel>(A<JobProfileTasksSegmentModel>.Ignored)).MustHaveHappened(resultsCount, Times.Exactly);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IndexViewModel>(viewResult.ViewData.Model);

            A.Equals(resultsCount, model.Documents.Count());

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async void ReturnsSuccessWhenNoData(string mediaTypeName)
        {
            // Arrange
            const int resultsCount = 0;
            IEnumerable<JobProfileTasksSegmentModel> expectedResults = null;
            var controller = BuildSegmentController(mediaTypeName);

            A.CallTo(() => JobProfileSegmentService.GetAllAsync()).Returns(expectedResults);
            A.CallTo(() => Mapper.Map<IndexDocumentViewModel>(A<JobProfileTasksSegmentModel>.Ignored)).Returns(A.Fake<IndexDocumentViewModel>());

            // Act
            var result = await controller.Index().ConfigureAwait(false);

            // Assert
            A.CallTo(() => JobProfileSegmentService.GetAllAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => Mapper.Map<IndexDocumentViewModel>(A<JobProfileTasksSegmentModel>.Ignored)).MustHaveHappened(resultsCount, Times.Exactly);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IndexViewModel>(viewResult.ViewData.Model);

            A.Equals(null, model.Documents);

            controller.Dispose();
        }
    }
}
