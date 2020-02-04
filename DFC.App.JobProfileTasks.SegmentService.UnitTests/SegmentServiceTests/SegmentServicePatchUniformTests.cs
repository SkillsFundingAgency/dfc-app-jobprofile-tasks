using AutoMapper;
using DFC.App.JobProfileTasks.Data.Enums;
using DFC.App.JobProfileTasks.Data.Models.PatchModels;
using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using DFC.App.JobProfileTasks.Data.Models.ServiceBusModels;
using DFC.App.JobProfileTasks.Repository.CosmosDb;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Uniform = DFC.App.JobProfileTasks.Data.Models.SegmentModels.Uniform;

namespace DFC.App.JobProfileTasks.SegmentService.UnitTests.SegmentServiceTests
{
    public class SegmentServicePatchUniformTests
    {
        private readonly Guid jobProfileId = Guid.NewGuid();
        private readonly Guid uniformId = Guid.NewGuid();
        private readonly ICosmosRepository<JobProfileTasksSegmentModel> repository;
        private readonly IMapper mapper;
        private readonly IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> jobProfileSegmentRefreshService;

        public SegmentServicePatchUniformTests()
        {
            repository = A.Fake<ICosmosRepository<JobProfileTasksSegmentModel>>();
            mapper = A.Fake<IMapper>();
            jobProfileSegmentRefreshService = A.Fake<IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel>>();
        }

        [Fact]
        public async Task PatchUniformReturnsArgumentNullExceptionWhenModelIsNull()
        {
            // Arrange
            var segmentService = new JobProfileTasksSegmentService(repository, mapper, jobProfileSegmentRefreshService);

            // Act
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await segmentService.PatchUniformAsync(null, Guid.NewGuid()).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact]
        public async Task PatchUniformReturnsNotFoundWhenDataDoesNotExist()
        {
            // Arrange
            var fakeRepository = A.Fake<ICosmosRepository<JobProfileTasksSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).Returns((JobProfileTasksSegmentModel)null);
            var segmentService = new JobProfileTasksSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchUniformAsync(GetPatchUniformModel(), jobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.NotFound, result);
        }

        [Fact]
        public async Task PatchUniformReturnsAlreadyReportedWhenExistingSequenceNumberIsHigher()
        {
            // Arrange
            var existingModel = GetJobProfileTasksSegmentModel(999);

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileTasksSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            var segmentService = new JobProfileTasksSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchUniformAsync(GetPatchUniformModel(), jobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.AlreadyReported, result);
        }

        [Fact]
        public async Task PatchUniformReturnsNotFoundWhenMessageActionIsPublishedAndDataDoesNotExist()
        {
            // Arrange
            var existingModel = GetJobProfileTasksSegmentModel();
            var patchModel = GetPatchUniformModel(patchUniformId: Guid.NewGuid());

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileTasksSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            var segmentService = new JobProfileTasksSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchUniformAsync(patchModel, jobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.NotFound, result);
        }

        [Fact]
        public async Task PatchUniformReturnsAlreadyReportedWhenMessageActionIsDeletedAndDataDoesNotExist()
        {
            // Arrange
            var existingModel = GetJobProfileTasksSegmentModel();
            var patchModel = GetPatchUniformModel(MessageActionType.Deleted, Guid.NewGuid());

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileTasksSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            var segmentService = new JobProfileTasksSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchUniformAsync(patchModel, jobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.AlreadyReported, result);
        }

        [Fact]
        public async Task PatchUniformReturnsSuccessWhenMessageActionIsPublished()
        {
            // Arrange
            var model = GetPatchUniformModel();
            var existingModel = GetJobProfileTasksSegmentModel();

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileTasksSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            A.CallTo(() => fakeRepository.UpsertAsync(A<JobProfileTasksSegmentModel>.Ignored)).Returns(HttpStatusCode.OK);

            var segmentService = new JobProfileTasksSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchUniformAsync(model, jobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeRepository.UpsertAsync(A<JobProfileTasksSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => jobProfileSegmentRefreshService.SendMessageAsync(A<RefreshJobProfileSegmentServiceBusModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<Uniform>(A<PatchUniformModel>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task PatchUniformReturnsSuccessWhenMessageActionIsDeleted()
        {
            // Arrange
            var model = GetPatchUniformModel(MessageActionType.Deleted);
            var existingModel = GetJobProfileTasksSegmentModel();

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileTasksSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            A.CallTo(() => fakeRepository.UpsertAsync(A<JobProfileTasksSegmentModel>.Ignored)).Returns(HttpStatusCode.OK);

            var segmentService = new JobProfileTasksSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchUniformAsync(model, jobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeRepository.UpsertAsync(A<JobProfileTasksSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => jobProfileSegmentRefreshService.SendMessageAsync(A<RefreshJobProfileSegmentServiceBusModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<Uniform>(A<PatchUniformModel>.Ignored)).MustNotHaveHappened();
            Assert.Equal(HttpStatusCode.OK, result);
        }

        private JobProfileTasksSegmentModel GetJobProfileTasksSegmentModel(int sequenceNumber = 1)
        {
            return new JobProfileTasksSegmentModel
            {
                DocumentId = jobProfileId,
                SequenceNumber = sequenceNumber,
                Data = new JobProfileTasksDataSegmentModel
                {
                    Uniforms = new List<Uniform>
                    {
                        new Uniform
                        {
                            Id = uniformId,
                            Description = "Uniform description",
                            IsNegative = true,
                            Title = "Uniform title",
                            Url = "/someurl",
                        },
                    },
                },
            };
        }

        private PatchUniformModel GetPatchUniformModel(MessageActionType messageAction = MessageActionType.Published, Guid? patchUniformId = null)
        {
            return new PatchUniformModel
            {
                JobProfileId = jobProfileId,
                Id = patchUniformId ?? uniformId,
                Title = "Amended Uniform",
                MessageAction = messageAction,
                SequenceNumber = 123,
                Description = "Amended uniform description",
                IsNegative = true,
                Url = "/someAmendedUrl",
            };
        }
    }
}