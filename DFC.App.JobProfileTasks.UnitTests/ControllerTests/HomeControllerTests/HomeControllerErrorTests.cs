using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DFC.App.JobProfileTasks.UnitTests.ControllerTests.HomeControllerTests
{
    public class HomeControllerErrorTests : BaseHomeController
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public void HomeControllerErrorHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            var controller = BuildHomeController(mediaTypeName);

            // Act
            var result = controller.Error();

            // Assert
            Assert.IsType<ViewResult>(result);

            controller.Dispose();
        }
    }
}
