using DFC.App.JobProfileTasks.Common.Constants;
using DFC.App.JobProfileTasks.Common.Contracts;
using Microsoft.AspNetCore.Http;

namespace DFC.App.JobProfileTasks.Common.Services
{
    public class RequestHeaderCorrelationIdProvider : ICorrelationIdProvider
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public RequestHeaderCorrelationIdProvider(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public string CorrelationId
        {
            get => httpContextAccessor.HttpContext.Request.Headers[HeaderName.CorrelationId];
            set => httpContextAccessor.HttpContext.Request.Headers[HeaderName.CorrelationId] = value;
        }
    }
}