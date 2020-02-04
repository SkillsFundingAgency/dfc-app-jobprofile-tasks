using AutoMapper;
using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using DFC.App.JobProfileTasks.Data.Models.ServiceBusModels;
using DFC.App.JobProfileTasks.MessageFunctionApp.AutoMapperProfiles;
using DFC.App.JobProfileTasks.MessageFunctionApp.Services;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xunit;
using Environment = DFC.App.JobProfileTasks.Data.Models.ServiceBusModels.Environment;
using Location = DFC.App.JobProfileTasks.Data.Models.ServiceBusModels.Location;
using Uniform = DFC.App.JobProfileTasks.Data.Models.ServiceBusModels.Uniform;

namespace DFC.App.JobProfileTasks.MessageFunctionApp.UnitTests.Services
{
    public class MappingServiceTests
    {
        private const int SequenceNumber = 123;
        private const string TestJobName = "Test Job name";
        private const string SocCodeId = "99";
        private const string DailyTask = "Daily Task 1";
        private const string Introduction = "Introduction 1";
        private const string UniformDescription1 = "UniformDescription 1";
        private const string UniformTitle1 = "UniformTitle 1";
        private const string UniformUrl1 = "UniformUrl 1";
        private const string LocationDescription1 = "LocationDescription 1";
        private const string LocationTitle1 = "LocationTitle 1";
        private const string LocationUrl1 = "LocationUrl 1";
        private const string EnvironmentDescription1 = "EnvironmentDescription 1";
        private const string EnvironmentTitle1 = "EnvironmentTitle 1";
        private const string EnvironmentUrl1 = "EnvironmentUrl 1";

        private static readonly DateTime LastModified = DateTime.UtcNow.AddDays(-1);
        private static readonly Guid JobProfileId = Guid.NewGuid();
        private static readonly Guid UniformId1 = Guid.NewGuid();
        private static readonly Guid LocationId1 = Guid.NewGuid();
        private static readonly Guid EnvironmentId1 = Guid.NewGuid();

        private readonly IMappingService mappingService;

        public MappingServiceTests()
        {
            var config = new MapperConfiguration(opts =>
            {
                opts.AddProfile(new TasksProfile());
            });

            var mapper = new Mapper(config);

            mappingService = new MappingService(mapper);
        }

        [Fact]
        public void MapToSegmentModelWhenJobProfileMessageSentThenItIsMappedCorrectly()
        {
            // Arrange
            var fullJPMessage = BuildJobProfileMessage();
            var message = JsonConvert.SerializeObject(fullJPMessage);
            var expectedResponse = BuildExpectedResponse();

            // Act
            var actualMappedModel = mappingService.MapToSegmentModel(message, SequenceNumber);

            // Assert
            expectedResponse.Should().BeEquivalentTo(actualMappedModel);
        }

        private JobProfileMessage BuildJobProfileMessage()
        {
            return new JobProfileMessage
            {
                JobProfileId = JobProfileId,
                CanonicalName = TestJobName,
                LastModified = LastModified,
                SOCLevelTwo = SocCodeId,
                WhatYouWillDoData = new JobProfileTasksDataServiceBusModel
                {
                    DailyTasks = DailyTask,
                    Introduction = Introduction,
                    Uniforms = new List<Uniform>
                    {
                        new Uniform
                        {
                            Id = UniformId1,
                            Description = UniformDescription1,
                            Title = UniformTitle1,
                            IsNegative = true,
                            Url = UniformUrl1,
                        },
                    },
                    Locations = new List<Location>
                    {
                        new Location
                        {
                            Id = LocationId1,
                            Description = LocationDescription1,
                            Title = LocationTitle1,
                            IsNegative = true,
                            Url = LocationUrl1,
                        },
                    },
                    Environments = new List<Environment>
                    {
                        new Environment
                        {
                            Id = EnvironmentId1,
                            Description = EnvironmentDescription1,
                            Title = EnvironmentTitle1,
                            IsNegative = true,
                            Url = EnvironmentUrl1,
                        },
                    },
                },
            };
        }

        private JobProfileTasksSegmentModel BuildExpectedResponse()
        {
            return new JobProfileTasksSegmentModel
            {
                CanonicalName = TestJobName,
                DocumentId = JobProfileId,
                SequenceNumber = SequenceNumber,
                SocLevelTwo = SocCodeId,
                Etag = null,
                Data = new JobProfileTasksDataSegmentModel
                {
                    Introduction = Introduction,
                    LastReviewed = LastModified,
                    Tasks = DailyTask,
                    Uniforms = new List<Data.Models.SegmentModels.Uniform>
                    {
                        new Data.Models.SegmentModels.Uniform
                        {
                            Id = UniformId1,
                            Description = UniformDescription1,
                            Title = UniformTitle1,
                            IsNegative = true,
                            Url = UniformUrl1,
                        },
                    },
                    Locations = new List<Data.Models.SegmentModels.Location>
                    {
                        new Data.Models.SegmentModels.Location
                        {
                            Id = LocationId1,
                            Description = LocationDescription1,
                            Title = LocationTitle1,
                            IsNegative = true,
                            Url = LocationUrl1,
                        },
                    },
                    Environments = new List<Data.Models.SegmentModels.Environment>
                    {
                        new Data.Models.SegmentModels.Environment
                        {
                            Id = EnvironmentId1,
                            Description = EnvironmentDescription1,
                            Title = EnvironmentTitle1,
                            IsNegative = true,
                            Url = EnvironmentUrl1,
                        },
                    },
                },
            };
        }
    }
}