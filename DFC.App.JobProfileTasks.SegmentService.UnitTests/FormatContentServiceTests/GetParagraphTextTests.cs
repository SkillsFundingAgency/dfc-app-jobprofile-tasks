using System.Collections.Generic;
using Xunit;

namespace DFC.App.JobProfileTasks.SegmentService.UnitTests.FormatContentServiceTests
{
    public class GetParagraphTextTests
    {
        [Fact]
        public void JoinSingleItemsUsingSeperator()
        {
            var service = new FormatContentService();
            var openingText = "openingText1";
            var seperator = ",";
            var items = new List<string>() { "Item1" };

            var result = service.GetParagraphText(openingText, items, seperator);

            Assert.Equal("openingText1 Item1.", result);
        }

        [Fact]
        public void Join2ItemsUsingSeperator()
        {
            var service = new FormatContentService();
            var openingText = "openingText1";
            var seperator = ",";
            var items = new List<string>() { "Item1", "Item2" };

            var result = service.GetParagraphText(openingText, items, seperator);

            Assert.Equal("openingText1 Item1 , Item2.", result);
        }

        [Fact]
        public void JoinMoreThan2ItemsItemsUsingSeperator()
        {
            var service = new FormatContentService();
            var openingText = "openingText1";
            var seperator = ",";
            var items = new List<string>() { "Item1", "Item2", "Item3" };

            var result = service.GetParagraphText(openingText, items, seperator);

            Assert.Equal("openingText1 Item1, Item2 , Item3.", result);
        }
    }
}
