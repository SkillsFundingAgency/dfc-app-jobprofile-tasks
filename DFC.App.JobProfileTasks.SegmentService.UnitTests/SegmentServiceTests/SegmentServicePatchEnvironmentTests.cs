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
using Environment = DFC.App.JobProfileTasks.Data.Models.SegmentModels.Environment;

namespace DFC.App.JobProfileTasks.SegmentService.UnitTests.SegmentServiceTests
{
    public class SegmentServicePatchEnvironmentTests
    {
        private readonly Guid jobProfileId = Guid.NewGuid();
        private readonly Guid environmentId = Guid.NewGuid();
        private readonly ICosmosRepository<JobProfileTasksSegmentModel> repository;
        private readonly IMapper mapper;
        private readonly IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> jobProfileSegmentRefreshService;

        public SegmentServicePatchEnvironmentTests()
        {
            repository = A.Fake<ICosmosRepository<JobProfileTasksSegmentModel>>();
            mapper = A.Fake<IMapper>();
            jobProfileSegmentRefreshService = A.Fake<IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel>>();
        }

        [Fact]
        public async Task PatchEnvironmentReturnsArgumentNullExceptionWhenModelIsNull()
        {
            // Arrange
            var segmentService = new JobProfileTasksSegmentService(repository, mapper, jobProfileSegmentRefreshService);

            // Act
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await segmentService.PatchEnvironmentAsync(null, Guid.NewGuid()).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact]
        public async Task PatchEnvironmentReturnsNotFoundWhenDataDoesNotExist()
        {
            // Arrange
            var fakeRepository = A.Fake<ICosmosRepository<JobProfileTasksSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).Returns((JobProfileTasksSegmentModel)null);
            var segmentService = new JobProfileTasksSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchEnvironmentAsync(GetPatchEnvironmentsModel(), jobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.NotFound, result);
        }

        [Fact]
        public async Task PatchEnvironmentReturnsAlreadyReportedWhenExistingSequenceNumberIsHigher()
        {
            // Arrange
            var existingModel = GetJobProfileTasksSegmentModel(999);

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileTasksSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            var segmentService = new JobProfileTasksSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchEnvironmentAsync(GetPatchEnvironmentsModel(), jobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.AlreadyReported, result);
        }

        [Fact]
        public async Task PatchEnvironmentReturnsNotFoundWhenMessageActionIsPublishedAndDataDoesNotExist()
        {
            // Arrange
            var existingModel = GetJobProfileTasksSegmentModel();
            var patchModel = GetPatchEnvironmentsModel(patchEnvironmentId: Guid.NewGuid());

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileTasksSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            var segmentService = new JobProfileTasksSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchEnvironmentAsync(patchModel, jobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.NotFound, result);
        }

        [Fact]
        public async Task PatchEnvironmentReturnsAlreadyReportedWhenMessageActionIsDeletedAndDataDoesNotExist()
        {
            // Arrange
            var existingModel = GetJobProfileTasksSegmentModel();
            var patchModel = GetPatchEnvironmentsModel(MessageActionType.Deleted, Guid.NewGuid());

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileTasksSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            var segmentService = new JobProfileTasksSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchEnvironmentAsync(patchModel, jobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.AlreadyReported, result);
        }

        [Fact]
        public async Task PatchEnvironmentReturnsSuccessWhenMessageActionIsPublished()
        {
            // Arrange
            var model = GetPatchEnvironmentsModel();
            var existingModel = GetJobProfileTasksSegmentModel();

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileTasksSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            A.CallTo(() => fakeRepository.UpsertAsync(A<JobProfileTasksSegmentModel>.Ignored)).Returns(HttpStatusCode.OK);

            var segmentService = new JobProfileTasksSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchEnvironmentAsync(model, jobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeRepository.UpsertAsync(A<JobProfileTasksSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => jobProfileSegmentRefreshService.SendMessageAsync(A<RefreshJobProfileSegmentServiceBusModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<Environment>(A<PatchEnvironmentsModel>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task PatchEnvironmentReturnsSuccessWhenMessageActionIsDeleted()
        {
            // Arrange
            var model = GetPatchEnvironmentsModel(MessageActionType.Deleted);
            var existingModel = GetJobProfileTasksSegmentModel();

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileTasksSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            A.CallTo(() => fakeRepository.UpsertAsync(A<JobProfileTasksSegmentModel>.Ignored)).Returns(HttpStatusCode.OK);

            var segmentService = new JobProfileTasksSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchEnvironmentAsync(model, jobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeRepository.UpsertAsync(A<JobProfileTasksSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => jobProfileSegmentRefreshService.SendMessageAsync(A<RefreshJobProfileSegmentServiceBusModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<Environment>(A<PatchEnvironmentsModel>.Ignored)).MustNotHaveHappened();
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
                    Environments = new List<Environment>
                    {
                        new Environment
                        {
                            Id = environmentId,
                            Description = "Environment description",
                            IsNegative = true,
                            Title = "Environment title",
                            Url = "/someurl",
                        },
                    },
                },
            };
        }

        private PatchEnvironmentsModel GetPatchEnvironmentsModel(MessageActionType messageAction = MessageActionType.Published, Guid? patchEnvironmentId = null)
        {
            return new PatchEnvironmentsModel
            {
                JobProfileId = jobProfileId,
                Id = patchEnvironmentId ?? environmentId,
                Title = "Amended environment",
                MessageAction = messageAction,
                SequenceNumber = 123,
                Description = "Amended environment description",
                IsNegative = true,
                Url = "/someAmendedUrl",
            };
        }
    }
}