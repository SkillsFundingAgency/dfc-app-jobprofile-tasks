using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using DFC.App.JobProfileTasks.IntegrationTests.Data;
using FluentAssertions;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileTasks.IntegrationTests.ControllerTests
{
    public class SegmentControllerRoutePostTests :
        IClassFixture<CustomWebApplicationFactory<Startup>>,
        IClassFixture<DataSeeding>
    {
        private readonly CustomWebApplicationFactory<Startup> factory;
        private readonly DataSeeding dataSeeding;

        public SegmentControllerRoutePostTests(
            CustomWebApplicationFactory<Startup> factory,
            DataSeeding dataSeeding)
        {
            this.factory = factory;
            this.dataSeeding = dataSeeding ?? throw new ArgumentNullException(nameof(dataSeeding));
            dataSeeding.AddData(factory).Wait();
        }

        [Fact]
        public async Task WhenAddingNewArticleReturnCreated()
        {
            // Arrange
            var url = "/segment";
            var documentId = Guid.NewGuid();
            var tasksSegmentModel = new JobProfileTasksSegmentModel()
            {
                DocumentId = documentId,
                CanonicalName = documentId.ToString().ToLowerInvariant(),
                SocLevelTwo = "12PostSoc",
                Data = new JobProfileTasksDataSegmentModel(),
            };
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            // Act
            var response = await client.PostAsync(url, tasksSegmentModel, new JsonMediaTypeFormatter()).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            // Cleanup
            await client.DeleteAsync(string.Concat(url, "/", documentId)).ConfigureAwait(false);
        }

        [Fact]
        public async Task WhenUpdateExistingArticleReturnsOK()
        {
            // Arrange
            const string url = "/segment";
            var tasksSegmentModel = new JobProfileTasksSegmentModel()
            {
                DocumentId = dataSeeding.Article2Id,
                CanonicalName = "article2_modified",
                SocLevelTwo = dataSeeding.Article2SocCode,
                Data = new JobProfileTasksDataSegmentModel(),
            };
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            // Act
            var response = await client.PostAsync(url, tasksSegmentModel, new JsonMediaTypeFormatter()).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }
    }
}