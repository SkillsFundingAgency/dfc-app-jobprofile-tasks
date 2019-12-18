using DFC.HtmlToDataTranslator.Contracts;
using FakeItEasy;
using System.Collections.Generic;
using Xunit;

namespace DFC.App.JobProfileTasks.SegmentService.UnitTests.FormatContentServiceTests
{
    public class GetParagraphTextTests
    {
        private const string OpeningText = "openingText1";
        private const string Seperator = ",";

        [Fact]
        public void WhenDataItemsAreEmptyThenEmptyStringReturned()
        {
            // Arrange
            var service = new FormatContentService();
            var items = new List<string>();

            // Act
            var result = service.GetParagraphText(OpeningText, items, Seperator, null);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void JoinSingleItemsUsingSeperator()
        {
            // Arrange
            var service = new FormatContentService();
            var items = new List<string> { "Item1" };

            // Act
            var result = service.GetParagraphText(OpeningText, items, Seperator, null);

            // Assert
            Assert.Equal("openingText1 Item1.", result);
        }

        [Fact]
        public void Join2ItemsUsingSeperator()
        {
            // Arrange
            var service = new FormatContentService();
            var items = new List<string> { "Item1", "Item2" };

            // Act
            var result = service.GetParagraphText(OpeningText, items, Seperator, null);

            // Assert
            Assert.Equal("openingText1 Item1 , Item2.", result);
        }

        [Fact]
        public void JoinMoreThan2ItemsItemsUsingSeperator()
        {
            // Arrange
            var service = new FormatContentService();
            var items = new List<string> { "Item1", "Item2", "Item3" };

            // Act
            var result = service.GetParagraphText(OpeningText, items, Seperator, null);

            // Assert
            Assert.Equal("openingText1 Item1, Item2 , Item3.", result);
        }

        [Fact]
        public void WhenTranslatorIsNotNullThenSourceItemsTranslated()
        {
            // Arrange
            var service = new FormatContentService();
            var translator = A.Fake<IHtmlToDataTranslator>();
            var items = new List<string> { "Item1", "Item2", "Item3" };
            const string expectedResult = "openingText1 Item1, translated_Item1, Item2, translated_Item2, Item3 , translated_Item3.";

            foreach (var item in items)
            {
                A.CallTo(() => translator.Translate(item)).Returns(new List<string> { item, $"translated_{item}" });
            }

            // Act
            var result = service.GetParagraphText(OpeningText, items, Seperator, translator);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}