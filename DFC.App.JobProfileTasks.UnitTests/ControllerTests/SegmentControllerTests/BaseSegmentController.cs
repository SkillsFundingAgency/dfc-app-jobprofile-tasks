using DFC.App.JobProfileTasks.Controllers;
using DFC.App.JobProfileTasks.Data.Contracts;
using DFC.App.JobProfileTasks.SegmentService;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Net.Mime;

namespace DFC.App.JobProfileTasks.UnitTests.ControllerTests.SegmentControllerTests
{
    public abstract class BaseSegmentController
    {
        public BaseSegmentController()
        {
            Logger = A.Fake<ILogger<SegmentController>>();
            JobProfileSegmentService = A.Fake<IJobProfileTasksSegmentService>();
            Mapper = A.Fake<AutoMapper.IMapper>();
            FormatContentService = A.Fake<IFormatContentService>();
        }

        public static IEnumerable<object[]> HtmlMediaTypes => new List<object[]>
        {
            new string[] { "*/*" },
            new string[] { MediaTypeNames.Text.Html },
        };

        public static IEnumerable<object[]> InvalidMediaTypes => new List<object[]>
        {
            new string[] { MediaTypeNames.Text.Plain },
        };

        public static IEnumerable<object[]> JsonMediaTypes => new List<object[]>
        {
            new string[] { MediaTypeNames.Application.Json },
        };

        protected ILogger<SegmentController> Logger { get; }

        protected IJobProfileTasksSegmentService JobProfileSegmentService { get; }

        protected AutoMapper.IMapper Mapper { get; }

        protected IFormatContentService FormatContentService { get; }

        protected SegmentController BuildSegmentController(string mediaTypeName)
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.Accept] = mediaTypeName;

            var controller = new SegmentController(Logger, JobProfileSegmentService, Mapper, FormatContentService)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                },
            };

            return controller;
        }
    }
}
