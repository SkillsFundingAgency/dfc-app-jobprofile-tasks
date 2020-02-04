using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using DFC.App.JobProfileTasks.Data.Models.ServiceBusModels;
using DFC.App.JobProfileTasks.Repository.CosmosDb;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileTasks.SegmentService.UnitTests.SegmentServiceTests
{
    public class SegmentServiceDeleteTests
    {
        private readonly ICosmosRepository<JobProfileTasksSegmentModel> repository;
        private readonly IJobProfileTasksSegmentService tasksSegmentService;

        private readonly Guid documentId = Guid.NewGuid();

        public SegmentServiceDeleteTests()
        {
            var mapper = A.Fake<AutoMapper.IMapper>();
            var jobProfileSegmentRefreshService = A.Fake<IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel>>();
            repository = A.Fake<ICosmosRepository<JobProfileTasksSegmentModel>>();
            tasksSegmentService = new JobProfileTasksSegmentService(repository, mapper, jobProfileSegmentRefreshService);
        }

        public static IEnumerable<object[]> InvalidStatusCodes => new List<object[]>
        {
            new object[] { HttpStatusCode.BadRequest },
            new object[] { HttpStatusCode.AlreadyReported },
            new object[] { HttpStatusCode.OK },
        };

        [Fact]
        public async Task DeleteReturnsSuccessWhenSegmentDeleted()
        {
            // arrange
            A.CallTo(() => repository.DeleteAsync(documentId)).Returns(HttpStatusCode.NoContent);

            // act
            var result = await tasksSegmentService.DeleteAsync(documentId).ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.DeleteAsync(documentId)).MustHaveHappenedOnceExactly();
            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(InvalidStatusCodes))]
        public async Task DeleteReturnsFalseWhenDocumentNotFound(HttpStatusCode statusCode)
        {
            // arrange
            A.CallTo(() => repository.DeleteAsync(documentId)).Returns(statusCode);

            // act
            var result = await tasksSegmentService.DeleteAsync(documentId).ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.DeleteAsync(documentId)).MustHaveHappenedOnceExactly();
            Assert.False(result);
        }
    }
}