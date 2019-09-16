using DFC.App.JobProfileTasks.IntegrationTests.Data;
using FluentAssertions;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileTasks.IntegrationTests.ControllerTests
{
    public class SegmentControllerRouteDeleteTests : IClassFixture<CustomWebApplicationFactory<Startup>>, IClassFixture<DataSeeding>
    {
        private readonly CustomWebApplicationFactory<Startup> factory;
        private readonly DataSeeding dataSeeding;

        public SegmentControllerRouteDeleteTests(CustomWebApplicationFactory<Startup> factory, DataSeeding dataSeeding)
        {
            this.factory = factory;
            this.dataSeeding = dataSeeding ?? throw new ArgumentNullException(nameof(dataSeeding));
            dataSeeding.AddData(factory).Wait();
        }

        [Fact]
        public async Task NonExistingSegmentReturnsNotFound()
        {
            // Arrange
            var uri = new Uri($"/segment/{Guid.NewGuid()}", UriKind.Relative);
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            // Act
            var response = await client.DeleteAsync(uri).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ExistingSegmentReturnsOk()
        {
            // Arrange
            var uri = new Uri($"/segment/{dataSeeding.Article1Id}", UriKind.Relative);
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            // Act
            var response = await client.DeleteAsync(uri).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}