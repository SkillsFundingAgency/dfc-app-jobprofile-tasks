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
using Location = DFC.App.JobProfileTasks.Data.Models.SegmentModels.Location;

namespace DFC.App.JobProfileTasks.SegmentService.UnitTests.SegmentServiceTests
{
    public class SegmentServicePatchLocationTests
    {
        private readonly Guid jobProfileId = Guid.NewGuid();
        private readonly Guid locationId = Guid.NewGuid();
        private readonly ICosmosRepository<JobProfileTasksSegmentModel> repository;
        private readonly IMapper mapper;
        private readonly IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> jobProfileSegmentRefreshService;

        public SegmentServicePatchLocationTests()
        {
            repository = A.Fake<ICosmosRepository<JobProfileTasksSegmentModel>>();
            mapper = A.Fake<IMapper>();
            jobProfileSegmentRefreshService = A.Fake<IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel>>();
        }

        [Fact]
        public async Task PatchLocationReturnsArgumentNullExceptionWhenModelIsNull()
        {
            // Arrange
            var segmentService = new JobProfileTasksSegmentService(repository, mapper, jobProfileSegmentRefreshService);

            // Act
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await segmentService.PatchLocationAsync(null, Guid.NewGuid()).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact]
        public async Task PatchLocationReturnsNotFoundWhenDataDoesNotExist()
        {
            // Arrange
            var fakeRepository = A.Fake<ICosmosRepository<JobProfileTasksSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).Returns((JobProfileTasksSegmentModel)null);
            var segmentService = new JobProfileTasksSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchLocationAsync(GetPatchLocationModel(), jobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.NotFound, result);
        }

        [Fact]
        public async Task PatchLocationReturnsAlreadyReportedWhenExistingSequenceNumberIsHigher()
        {
            // Arrange
            var existingModel = GetJobProfileTasksSegmentModel(999);

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileTasksSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            var segmentService = new JobProfileTasksSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchLocationAsync(GetPatchLocationModel(), jobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.AlreadyReported, result);
        }

        [Fact]
        public async Task PatchLocationReturnsNotFoundWhenMessageActionIsPublishedAndDataDoesNotExist()
        {
            // Arrange
            var existingModel = GetJobProfileTasksSegmentModel();
            var patchModel = GetPatchLocationModel(patchLocationId: Guid.NewGuid());

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileTasksSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            var segmentService = new JobProfileTasksSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchLocationAsync(patchModel, jobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.NotFound, result);
        }

        [Fact]
        public async Task PatchLocationReturnsAlreadyReportedWhenMessageActionIsDeletedAndDataDoesNotExist()
        {
            // Arrange
            var existingModel = GetJobProfileTasksSegmentModel();
            var patchModel = GetPatchLocationModel(MessageActionType.Deleted, Guid.NewGuid());

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileTasksSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            var segmentService = new JobProfileTasksSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchLocationAsync(patchModel, jobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.AlreadyReported, result);
        }

        [Fact]
        public async Task PatchLocationReturnsSuccessWhenMessageActionIsPublished()
        {
            // Arrange
            var model = GetPatchLocationModel();
            var existingModel = GetJobProfileTasksSegmentModel();

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileTasksSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            A.CallTo(() => fakeRepository.UpsertAsync(A<JobProfileTasksSegmentModel>.Ignored)).Returns(HttpStatusCode.OK);

            var segmentService = new JobProfileTasksSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchLocationAsync(model, jobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeRepository.UpsertAsync(A<JobProfileTasksSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => jobProfileSegmentRefreshService.SendMessageAsync(A<RefreshJobProfileSegmentServiceBusModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<Location>(A<PatchLocationModel>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task PatchLocationReturnsSuccessWhenMessageActionIsDeleted()
        {
            // Arrange
            var model = GetPatchLocationModel(MessageActionType.Deleted);
            var existingModel = GetJobProfileTasksSegmentModel();

            var fakeRepository = A.Fake<ICosmosRepository<JobProfileTasksSegmentModel>>();
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).Returns(existingModel);
            A.CallTo(() => fakeRepository.UpsertAsync(A<JobProfileTasksSegmentModel>.Ignored)).Returns(HttpStatusCode.OK);

            var segmentService = new JobProfileTasksSegmentService(fakeRepository, mapper, jobProfileSegmentRefreshService);

            // Act
            var result = await segmentService.PatchLocationAsync(model, jobProfileId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeRepository.GetAsync(A<Expression<Func<JobProfileTasksSegmentModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeRepository.UpsertAsync(A<JobProfileTasksSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => jobProfileSegmentRefreshService.SendMessageAsync(A<RefreshJobProfileSegmentServiceBusModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<Location>(A<PatchLocationModel>.Ignored)).MustNotHaveHappened();
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
                    Locations = new List<Location>
                    {
                        new Location
                        {
                            Id = locationId,
                            Description = "Location description",
                            IsNegative = true,
                            Title = "Location title",
                            Url = "/someurl",
                        },
                    },
                },
            };
        }

        private PatchLocationModel GetPatchLocationModel(MessageActionType messageAction = MessageActionType.Published, Guid? patchLocationId = null)
        {
            return new PatchLocationModel
            {
                JobProfileId = jobProfileId,
                Id = patchLocationId ?? locationId,
                Title = "Amended Location",
                MessageAction = messageAction,
                SequenceNumber = 123,
                Description = "Amended Location description",
                IsNegative = true,
                Url = "/someAmendedUrl",
            };
        }
    }
}