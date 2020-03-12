using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using DFC.App.JobProfileTasks.Data.Models.ServiceBusModels;
using DFC.App.JobProfileTasks.Repository.CosmosDb;
using FakeItEasy;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileTasks.SegmentService.UnitTests.SegmentServiceTests
{
    public class SegmentServiceUpsertTests
    {
        private readonly ICosmosRepository<JobProfileTasksSegmentModel> repository;
        private readonly IJobProfileTasksSegmentService skillSegmentService;

        public SegmentServiceUpsertTests()
        {
            var jobProfileSegmentRefreshService = A.Fake<IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel>>();
            var mapper = A.Fake<AutoMapper.IMapper>();
            repository = A.Fake<ICosmosRepository<JobProfileTasksSegmentModel>>();
            skillSegmentService = new JobProfileTasksSegmentService(repository, mapper, jobProfileSegmentRefreshService);
        }

        [Fact]
        public async Task UpsertReturnsCreatedWhenDocumentCreated()
        {
            // Arrange
            var skillSegmentModel = A.Fake<JobProfileTasksSegmentModel>();
            var expectedResult = HttpStatusCode.Created;

            A.CallTo(() => repository.UpsertAsync(skillSegmentModel)).Returns(expectedResult);

            // Act
            var result = await skillSegmentService.UpsertAsync(skillSegmentModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => repository.UpsertAsync(skillSegmentModel)).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task UpsertReturnsSuccessWhenDocumentReplaced()
        {
            // Arrange
            var skillSegmentModel = A.Fake<JobProfileTasksSegmentModel>();
            var expectedResult = HttpStatusCode.OK;

            A.CallTo(() => repository.UpsertAsync(skillSegmentModel)).Returns(expectedResult);

            // Act
            var result = await skillSegmentService.UpsertAsync(skillSegmentModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => repository.UpsertAsync(skillSegmentModel)).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task UpsertReturnsArgumentNullExceptionWhenNullParamIsUsed()
        {
            // Act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await skillSegmentService.UpsertAsync(null).ConfigureAwait(false)).ConfigureAwait(false);

            // Assert
            Assert.Equal("Value cannot be null. (Parameter 'tasksSegmentModel')", exceptionResult.Message);
        }
    }
}