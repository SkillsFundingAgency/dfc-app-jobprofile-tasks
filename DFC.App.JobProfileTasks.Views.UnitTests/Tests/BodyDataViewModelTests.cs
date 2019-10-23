using DFC.App.JobProfileTasks.ViewModels;
using DFC.App.JobProfileTasks.Views.UnitTests.Services;
using System;
using System.Collections.Generic;
using Xunit;

namespace DFC.App.JobProfileTasks.Views.UnitTests.Tests
{
    public class BodyDataViewModelTests : TestBase
    {
        [Fact]
        public void ViewContainsRenderedContent()
        {
            //Arrange
            var model = new BodyDataViewModel()
            {
                Environment = "Environment1",
                Location = "Locations1",
                DailyTasks = "Tasks1",
                Uniform = "Uniform1",
            };

            var viewBag = new Dictionary<string, object>();
            var viewRenderer = new RazorEngineRenderer(ViewRootPath);

            //Act
            var viewRenderResponse = viewRenderer.Render(@"BodyData", model, viewBag);

            //Assert
            Assert.Contains(model.Environment, viewRenderResponse, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(model.Location, viewRenderResponse, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(model.DailyTasks, viewRenderResponse, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(model.Uniform, viewRenderResponse, StringComparison.OrdinalIgnoreCase);
        }
    }
}
