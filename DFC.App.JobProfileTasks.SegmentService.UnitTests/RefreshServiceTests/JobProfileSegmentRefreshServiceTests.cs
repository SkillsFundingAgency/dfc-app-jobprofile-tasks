using DFC.App.JobProfileTasks.Data.Models.ServiceBusModels;
using FakeItEasy;
using Microsoft.Azure.ServiceBus;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileTasks.SegmentService.UnitTests.RefreshServiceTests
{
    public class JobProfileSegmentRefreshServiceTests
    {
        [Fact]
        public async Task SendMessageSendsMessageOnTopicClient()
        {
            // Arrange
            var fakeTopicClient = A.Fake<ITopicClient>();
            var refreshService = new JobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel>(fakeTopicClient);

            var model = new RefreshJobProfileSegmentServiceBusModel
            {
                CanonicalName = "some-canonical-name-1",
                JobProfileId = Guid.NewGuid(),
                Segment = "WhatYouWillDo",
            };

            // Act
            await refreshService.SendMessageAsync(model).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeTopicClient.SendAsync(A<Message>.Ignored)).MustHaveHappenedOnceExactly();
        }
    }
}