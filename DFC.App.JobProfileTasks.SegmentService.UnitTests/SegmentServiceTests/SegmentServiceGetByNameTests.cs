using AutoMapper;
using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using DFC.App.JobProfileTasks.Data.Models.ServiceBusModels;
using DFC.App.JobProfileTasks.Repository.CosmosDb;
using FakeItEasy;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileTasks.SegmentService.UnitTests.SegmentServiceTests
{
    public class SegmentServiceGetByNameTests
    {
        private readonly ICosmosRepository<JobProfileTasksSegmentModel> repository;
        private readonly IJobProfileTasksSegmentService jobProfileTasksSegmentService;
        private readonly IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> jobProfileSegmentRefreshService;
        private readonly IMapper mapper;

        public SegmentServiceGetByNameTests()
        {
            repository = A.Fake<ICosmosRepository<JobProfileTasksSegmentModel>>();
            mapper = A.Fake<IMapper>();
            jobProfileSegmentRefreshService = A.Fake<IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel>>();
            jobProfileTasksSegmentService = new JobProfileTasksSegmentService(repository, mapper, jobProfileSegmentRefreshService);
        }

        [Fact]
        public async Task GetByNameReturnsSuccess()
        {
            // arrange
            var expectedResult = A.Fake<JobProfileTasksSegmentModel>();

            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = await jobProfileTasksSegmentService.GetByNameAsync("article-name").ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task GetByNameReturnsArgumentNullExceptionWhenNullIsUsed()
        {
            // arrange

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await jobProfileTasksSegmentService.GetByNameAsync(null).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal("Value cannot be null. (Parameter 'canonicalName')", exceptionResult.Message);
        }

        [Fact]
        public async Task GetByNameReturnsNullWhenMissingInRepository()
        {
            // arrange
            JobProfileTasksSegmentModel expectedResult = null;

            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = await jobProfileTasksSegmentService.GetByNameAsync("article-name").ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResult, result);
        }
    }
}