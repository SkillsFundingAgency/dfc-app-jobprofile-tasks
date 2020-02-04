using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using DFC.App.JobProfileTasks.MessageFunctionApp.HttpClientPolicies;
using DFC.App.JobProfileTasks.MessageFunctionApp.Services;
using DFC.App.JobProfileTasks.MessageFunctionApp.UnitTests.ClientHandlers;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfileTasks.MessageFunctionApp.UnitTests.Services
{
    public class HttpClientServicePostTests
    {
        private readonly ILogService logger;
        private readonly JobProfileClientOptions segmentClientOptions;
        private readonly ICorrelationIdProvider correlationIdProvider;

        public HttpClientServicePostTests()
        {
            logger = A.Fake<ILogService>();
            segmentClientOptions = new JobProfileClientOptions
            {
                BaseAddress = new Uri("https://somewhere.com", UriKind.Absolute),
                Timeout = TimeSpan.MinValue,
            };
            correlationIdProvider = A.Fake<ICorrelationIdProvider>();
        }

        [Fact]
        public async Task PostFullJobProfileAsyncReturnsOkStatusCodeForExistingId()
        {
            // arrange
            const HttpStatusCode expectedResult = HttpStatusCode.OK;
            var httpResponse = new HttpResponseMessage { StatusCode = expectedResult };
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler) { BaseAddress = segmentClientOptions.BaseAddress };
            var httpClientService = new HttpClientService(segmentClientOptions, httpClient, logger, correlationIdProvider);

            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            // act
            var result = await httpClientService.PostAsync(A.Fake<JobProfileTasksSegmentModel>()).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(expectedResult, result);

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }

        [Fact]
        public async Task PostFullJobProfileAsyncReturnsExceptionForBadStatus()
        {
            // arrange
            const HttpStatusCode expectedResult = HttpStatusCode.Forbidden;
            var httpResponse = new HttpResponseMessage { StatusCode = expectedResult, Content = new StringContent("bad Post") };
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler) { BaseAddress = segmentClientOptions.BaseAddress };
            var httpClientService = new HttpClientService(segmentClientOptions, httpClient, logger, correlationIdProvider);

            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            // act
            var exceptionResult = await Assert.ThrowsAsync<HttpRequestException>(async () => await httpClientService.PostAsync(A.Fake<JobProfileTasksSegmentModel>()).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal($"Response status code does not indicate success: {(int)expectedResult} ({expectedResult}).", exceptionResult.Message);

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }
    }
}