using DFC.App.JobProfileTasks.Data.Models;
using DFC.App.JobProfileTasks.Repository.CosmosDb;
using FakeItEasy;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileTasks.SegmentService.UnitTests.SegmentServiceTests
{
    public class SegmentServiceGetByIdTests
    {
        private readonly ICosmosRepository<JobProfileTasksSegmentModel> repository;
        private readonly IJobProfileTasksSegmentService jobProfileTasksSegmentService;

        public SegmentServiceGetByIdTests()
        {
            repository = A.Fake<ICosmosRepository<JobProfileTasksSegmentModel>>();
            jobProfileTasksSegmentService = new JobProfileTasksSegmentService(repository);
        }

        [Fact]
        public async Task GetByIdReturnsSuccess()
        {
            // arrange
            var documentId = Guid.NewGuid();
            var expectedResult = A.Fake<JobProfileTasksSegmentModel>();

            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = await jobProfileTasksSegmentService.GetByIdAsync(documentId).ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task GetByIdReturnsNullWhenMissingInRepository()
        {
            // arrange
            var documentId = Guid.NewGuid();
            JobProfileTasksSegmentModel expectedResult = null;

            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = await jobProfileTasksSegmentService.GetByIdAsync(documentId).ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResult, result);
        }
    }
}