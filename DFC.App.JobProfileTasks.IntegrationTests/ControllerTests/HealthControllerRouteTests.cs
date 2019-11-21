using DFC.App.JobProfileTasks.IntegrationTests.Data;
using System;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileTasks.IntegrationTests.ControllerTests
{
    public class HealthControllerRouteTests : IClassFixture<CustomWebApplicationFactory<Startup>>, IClassFixture<DataSeeding>
    {
        private readonly CustomWebApplicationFactory<Startup> factory;

        public HealthControllerRouteTests(CustomWebApplicationFactory<Startup> factory, DataSeeding seeding)
        {
            this.factory = factory;
            seeding?.AddData(factory).GetAwaiter().GetResult();
        }

        [Theory]
        [InlineData("health/ping")]
        public async Task GetHealthOkEndpointsReturnSuccess(string url)
        {
            // Arrange
            var uri = new Uri(url, UriKind.Relative);
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();

            // Act
            var response = await client.GetAsync(uri).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData(MediaTypeNames.Text.Html)]
        [InlineData(MediaTypeNames.Application.Json)]
        public async Task ReturnSuccessAndCorrectContentType(string mediaType)
        {
            // Arrange
            var uri = new Uri("health", UriKind.Relative);
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));

            // Act
            var response = await client.GetAsync(uri).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal($"{mediaType}; charset={Encoding.UTF8.WebName}", response.Content.Headers.ContentType.ToString());
        }
    }
}