using AutoMapper;
using DFC.App.JobProfileTasks.Data.Enums;
using DFC.App.JobProfileTasks.Data.Models.PatchModels;
using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using DFC.App.JobProfileTasks.Data.Models.ServiceBusModels.PatchModels;
using DFC.App.JobProfileTasks.MessageFunctionApp.Services;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Environment = DFC.App.JobProfileTasks.Data.Models.SegmentModels.Environment;

namespace DFC.App.JobProfileTasks.MessageFunctionApp.UnitTests.Services
{
    public class MessageProcessorTests
    {
        private const long SequenceNumber = 123;
        private const string BaseMessage = "Dummy Serialised Message";
        private const int InvalidEnumValue = 999;

        private readonly IMessageProcessor processor;
        private readonly IHttpClientService httpClientService;
        private readonly IMappingService mappingService;
        private readonly IMapper mapper;

        public MessageProcessorTests()
        {
            var expectedMappedModel = GetSegmentModel();
            mapper = A.Fake<IMapper>();

            mappingService = A.Fake<IMappingService>();
            A.CallTo(() => mappingService.MapToSegmentModel(A<string>.Ignored, A<long>.Ignored)).Returns(expectedMappedModel);

            httpClientService = A.Fake<IHttpClientService>();
            A.CallTo(() => httpClientService.DeleteAsync(A<Guid>.Ignored)).Returns(HttpStatusCode.OK);

            processor = new MessageProcessor(mapper, httpClientService, mappingService);
        }

        [Fact]
        public async Task ProcessAsyncReturnsInternalServerErrorWhenInvalidMessageContentTypeSent()
        {
            // Act
            var result = await processor
                .ProcessAsync(BaseMessage, SequenceNumber, (MessageContentType)InvalidEnumValue, MessageActionType.Published)
                .ConfigureAwait(false);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, result);
        }

        [Fact]
        public async Task ProcessAsyncArgumentOutOfRangeExceptionWhenInvalidMessageActionTypeSent()
        {
            await Assert.ThrowsAnyAsync<ArgumentOutOfRangeException>(async () => await processor.ProcessAsync(BaseMessage, SequenceNumber, MessageContentType.JobProfile, (MessageActionType)InvalidEnumValue).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact]
        public async Task ProcessAsyncCallsDeleteAsyncWhenDeletedMessageActionTypeSent()
        {
            // Act
            var result = await processor
                .ProcessAsync(BaseMessage, SequenceNumber, MessageContentType.JobProfile, MessageActionType.Deleted)
                .ConfigureAwait(false);

            // Assert
            A.CallTo(() => httpClientService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task ProcessAsyncCallsPutAsyncAndReturnsOkResultWhenDataExists()
        {
            // Arrange
            var putHttpClientService = A.Fake<IHttpClientService>();
            A.CallTo(() => putHttpClientService.PutAsync(A<JobProfileTasksSegmentModel>.Ignored)).Returns(HttpStatusCode.OK);

            var putMessageProcessor = new MessageProcessor(mapper, putHttpClientService, mappingService);

            // Act
            var result = await putMessageProcessor
                .ProcessAsync(BaseMessage, SequenceNumber, MessageContentType.JobProfile, MessageActionType.Published)
                .ConfigureAwait(false);

            // Assert
            A.CallTo(() => putHttpClientService.PutAsync(A<JobProfileTasksSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task ProcessAsyncCallsPostAsyncAndReturnsOkResultWhenDataDoesntExist()
        {
            // Arrange
            var postHttpClientService = A.Fake<IHttpClientService>();
            A.CallTo(() => postHttpClientService.PutAsync(A<JobProfileTasksSegmentModel>.Ignored)).Returns(HttpStatusCode.NotFound);
            A.CallTo(() => postHttpClientService.PostAsync(A<JobProfileTasksSegmentModel>.Ignored)).Returns(HttpStatusCode.OK);

            var postMessageProcessor = new MessageProcessor(mapper, postHttpClientService, mappingService);

            // Act
            var result = await postMessageProcessor
                .ProcessAsync(BaseMessage, SequenceNumber, MessageContentType.JobProfile, MessageActionType.Published)
                .ConfigureAwait(false);

            // Assert
            A.CallTo(() => postHttpClientService.PutAsync(A<JobProfileTasksSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => postHttpClientService.PostAsync(A<JobProfileTasksSegmentModel>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task ProcessAsyncCallsPatchAsyncWhenPatchEnvironmentContentTypeSent()
        {
            // Arrange
            var fakeMapper = A.Fake<IMapper>();
            var postMessageProcessor = new MessageProcessor(fakeMapper, httpClientService, mappingService);
            A.CallTo(() => fakeMapper.Map<PatchEnvironmentsModel>(A<PatchEnvironmentServiceBusModel>.Ignored)).Returns(A.Fake<PatchEnvironmentsModel>());
            A.CallTo(() => httpClientService.PatchAsync(A<PatchEnvironmentsModel>.Ignored, A<string>.Ignored)).Returns(HttpStatusCode.OK);

            // Act
            var result = await postMessageProcessor
                .ProcessAsync("{}", SequenceNumber, MessageContentType.Environment, MessageActionType.Published)
                .ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeMapper.Map<PatchEnvironmentsModel>(A<PatchEnvironmentServiceBusModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => httpClientService.PatchAsync(A<PatchEnvironmentsModel>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task ProcessAsyncCallsPatchAsyncWhenPatchLocationModelContentTypeSent()
        {
            // Arrange
            var fakeMapper = A.Fake<IMapper>();
            var postMessageProcessor = new MessageProcessor(fakeMapper, httpClientService, mappingService);
            A.CallTo(() => fakeMapper.Map<PatchLocationModel>(A<PatchLocationServiceBusModel>.Ignored)).Returns(A.Fake<PatchLocationModel>());
            A.CallTo(() => httpClientService.PatchAsync(A<PatchLocationModel>.Ignored, A<string>.Ignored)).Returns(HttpStatusCode.OK);

            // Act
            var result = await postMessageProcessor
                .ProcessAsync("{}", SequenceNumber, MessageContentType.Location, MessageActionType.Published)
                .ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeMapper.Map<PatchLocationModel>(A<PatchLocationServiceBusModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => httpClientService.PatchAsync(A<PatchLocationModel>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.OK, result);
        }

        [Fact]
        public async Task ProcessAsyncCallsPatchAsyncWhenPatchUniformModelContentTypeSent()
        {
            // Arrange
            var fakeMapper = A.Fake<IMapper>();
            var postMessageProcessor = new MessageProcessor(fakeMapper, httpClientService, mappingService);
            A.CallTo(() => fakeMapper.Map<PatchUniformModel>(A<PatchUniformServiceBusModel>.Ignored)).Returns(A.Fake<PatchUniformModel>());
            A.CallTo(() => httpClientService.PatchAsync(A<PatchUniformModel>.Ignored, A<string>.Ignored)).Returns(HttpStatusCode.OK);

            // Act
            var result = await postMessageProcessor
                .ProcessAsync("{}", SequenceNumber, MessageContentType.Uniform, MessageActionType.Published)
                .ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeMapper.Map<PatchUniformModel>(A<PatchUniformServiceBusModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => httpClientService.PatchAsync(A<PatchUniformModel>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.OK, result);
        }

        private JobProfileTasksSegmentModel GetSegmentModel()
        {
            return new JobProfileTasksSegmentModel
            {
                DocumentId = Guid.NewGuid(),
                SequenceNumber = 1,
                SocLevelTwo = "12",
                CanonicalName = "job-1",
                Data = new JobProfileTasksDataSegmentModel
                {
                    Uniforms = new List<Uniform>
                    {
                        new Uniform
                        {
                            Id = Guid.NewGuid(),
                            Description = "Uniform description",
                            IsNegative = true,
                            Title = "Uniform title",
                            Url = "/someurl",
                        },
                    },
                    Locations = new List<Location>
                    {
                        new Location
                        {
                            Id = Guid.NewGuid(),
                            Description = "Location description",
                            IsNegative = true,
                            Title = "Location title",
                            Url = "/someurl",
                        },
                    },
                    Environments = new List<Environment>
                    {
                        new Environment
                        {
                            Id = Guid.NewGuid(),
                            Description = "Environment description",
                            IsNegative = true,
                            Title = "Environment title",
                            Url = "/someurl",
                        },
                    },
                },
            };
        }
    }
}