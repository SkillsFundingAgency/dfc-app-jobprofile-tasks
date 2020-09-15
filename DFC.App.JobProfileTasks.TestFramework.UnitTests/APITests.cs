using DFC.App.RelatedCareers.Tests.IntegrationTests.API.Model.API;
using DFC.App.RelatedCareers.Tests.IntegrationTests.API.Model.Support;
using DFC.App.RelatedCareers.Tests.IntegrationTests.API.Support.API;
using DFC.App.RelatedCareers.Tests.IntegrationTests.API.Support.API.RestFactory.Interface;
using FakeItEasy;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.RelatedCareers.Tests.TestFramework.UnitTests
{
    public class APITests
    {
        private AppSettings appSettings;
        private IRestClientFactory restClientFactory;
        private IRestRequestFactory restRequestFactory;
        private IRelatedCareersAPI careerPathApi;
        private IRestClient restClient;
        private IRestRequest restRequest;

        public APITests()
        {
            this.appSettings = new AppSettings();
            this.restClientFactory = A.Fake<IRestClientFactory>();
            this.restRequestFactory = A.Fake<IRestRequestFactory>();
            this.restClient = A.Fake<IRestClient>();
            this.restRequest = A.Fake<IRestRequest>();
            A.CallTo(() => this.restClientFactory.Create(A<Uri>.Ignored)).Returns(this.restClient);
            A.CallTo(() => this.restRequestFactory.Create(A<string>.Ignored)).Returns(this.restRequest);
            this.careerPathApi = new RelatedCareersAPI(this.restClientFactory, this.restRequestFactory, this.appSettings);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task EmptyOrNullIdResultsInNullBeingReturned(string id)
        {
            Assert.Null(await this.careerPathApi.GetById(id).ConfigureAwait(true));
        }

        [Fact]
        public async Task SuccessfulGetRequest()
        {
            var apiResponse = new RestResponse<List<RelatedCareersResponse>>();
            apiResponse.StatusCode = HttpStatusCode.OK;
            A.CallTo(() => this.restClient.Execute<List<RelatedCareersResponse>>(A<IRestRequest>.Ignored)).Returns(apiResponse);
            var response = await this.careerPathApi.GetById("id").ConfigureAwait(false);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("Accept", "application/json")]
        public async Task AllRequestHeadersAreSet(string headerKey, string headerValue)
        {
            var response = await this.careerPathApi.GetById("id").ConfigureAwait(false);
            A.CallTo(() => this.restRequest.AddHeader(headerKey, headerValue)).MustHaveHappenedOnceExactly();
        }
    }
}