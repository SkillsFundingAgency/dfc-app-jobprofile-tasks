using DFC.App.JobProfileTasks.Data.Models;
using DFC.App.JobProfileTasks.Repository.CosmosDb;
using FakeItEasy;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileTasks.SegmentService.UnitTests.SegmentServiceTests
{
    public class SegmentServiceGetAllTests
    {
        private readonly ICosmosRepository<JobProfileTasksSegmentModel> repository;
        private readonly IJobProfileTasksSegmentService jobProfileTasksSegmentService;

        public SegmentServiceGetAllTests()
        {
            repository = A.Fake<ICosmosRepository<JobProfileTasksSegmentModel>>();
            jobProfileTasksSegmentService = new JobProfileTasksSegmentService(repository);
        }

        [Fact]
        public async Task GetAllListReturnsSuccess()
        {
            // arrange
            var expectedResults = A.CollectionOfFake<JobProfileTasksSegmentModel>(2);

            A.CallTo(() => repository.GetAllAsync()).Returns(expectedResults);

            // act
            var results = await jobProfileTasksSegmentService.GetAllAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAllAsync()).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResults, results);
        }

        [Fact]
        public async Task GetAllListReturnsNullWhenMissingRepository()
        {
            // arrange
            IEnumerable<JobProfileTasksSegmentModel> expectedResults = null;

            A.CallTo(() => repository.GetAllAsync()).Returns(expectedResults);

            // act
            var results = await jobProfileTasksSegmentService.GetAllAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAllAsync()).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResults, results);
        }
    }
}