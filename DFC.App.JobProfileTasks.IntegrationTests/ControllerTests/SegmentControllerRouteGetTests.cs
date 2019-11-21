using DFC.App.JobProfileTasks.IntegrationTests.Data;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileTasks.IntegrationTests.ControllerTests
{
    public class SegmentControllerRouteGetTests :
        IClassFixture<CustomWebApplicationFactory<Startup>>,
        IClassFixture<DataSeeding>
    {
        private readonly CustomWebApplicationFactory<Startup> factory;
        private readonly DataSeeding dataSeeding;

        public SegmentControllerRouteGetTests(CustomWebApplicationFactory<Startup> factory, DataSeeding dataSeeding)
        {
            this.factory = factory;
            this.dataSeeding = dataSeeding ?? throw new ArgumentNullException(nameof(dataSeeding));
            dataSeeding.AddData(factory).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task GetSegmentHtmlContentEndpointsReturnSuccessAndCorrectContentTypeForIndex()
        {
            // Arrange
            var url = "segment";
            var uri = new Uri(url, UriKind.Relative);
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Text.Html));

            // Act
            var response = await client.GetAsync(uri).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            AssertContentType(MediaTypeNames.Text.Html, response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task GetSegmentHtmlContentEndpointsReturnSuccessAndCorrectContentTypeForArticle()
        {
            // Arrange
            var url = $"segment/{dataSeeding.Article2Id}";
            var uri = new Uri(url, UriKind.Relative);
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Text.Html));

            // Act
            var response = await client.GetAsync(uri).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            AssertContentType(MediaTypeNames.Text.Html, response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task GetSegmentHtmlContentEndpointsReturnSuccessAndCorrectContentTypeForArticleContents()
        {
            // Arrange
            var url = $"segment/{dataSeeding.Article2Id}/contents";
            var uri = new Uri(url, UriKind.Relative);
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Text.Html));

            // Act
            var response = await client.GetAsync(uri).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            AssertContentType(MediaTypeNames.Text.Html, response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("segment/abc")]
        public async Task GetSegmentHtmlContentEndpointsReturnNoContent(string url)
        {
            // Arrange
            var uri = new Uri(url, UriKind.Relative);
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();

            // Act
            var response = await client.GetAsync(uri).ConfigureAwait(false);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        private void AssertContentType(string expectedContentType, string actualContentType)
        {
            Assert.Equal($"{expectedContentType}; charset={Encoding.UTF8.WebName}", actualContentType);
        }
    }
}