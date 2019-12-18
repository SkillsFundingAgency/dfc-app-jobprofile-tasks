using DFC.App.JobProfileTasks.ApiModels;
using DFC.App.JobProfileTasks.Common.Contracts;
using DFC.App.JobProfileTasks.Data.Models.PatchModels;
using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using DFC.App.JobProfileTasks.Extensions;
using DFC.App.JobProfileTasks.SegmentService;
using DFC.App.JobProfileTasks.ViewModels;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.Controllers
{
    public class SegmentController : Controller
    {
        private const string IndexActionName = nameof(Index);
        private const string DocumentActionName = nameof(Document);
        private const string BodyActionName = nameof(Body);
        private const string PutActionName = nameof(Put);
        private const string PostActionName = nameof(Post);
        private const string DeleteActionName = nameof(Delete);
        private const string PatchUniformActionName = nameof(PatchUniform);
        private const string PatchLocationActionName = nameof(PatchLocation);
        private const string PatchEnvironmentActionName = nameof(PatchEnvironment);

        private readonly IJobProfileTasksSegmentService jobProfileTasksSegmentService;
        private readonly AutoMapper.IMapper mapper;
        private readonly ILogService logService;

        public SegmentController(IJobProfileTasksSegmentService jobProfileTasksSegmentService, AutoMapper.IMapper mapper, ILogService logService)
        {
            this.jobProfileTasksSegmentService = jobProfileTasksSegmentService;
            this.mapper = mapper;
            this.logService = logService;
        }

        [HttpGet]
        [Route("/")]
        [Route("{controller}")]
        public async Task<IActionResult> Index()
        {
            logService.LogMessage($"{IndexActionName} has been called");

            var viewModel = new IndexViewModel();
            var segmentModels = await jobProfileTasksSegmentService.GetAllAsync().ConfigureAwait(false);

            if (segmentModels != null)
            {
                viewModel.Documents = segmentModels
                    .OrderBy(x => x.CanonicalName)
                    .Select(x => mapper.Map<IndexDocumentViewModel>(x))
                    .ToList();

                logService.LogMessage($"{IndexActionName} has succeeded");
            }
            else
            {
                logService.LogMessage($"{IndexActionName} has returned with no results", SeverityLevel.Warning);
            }

            return View(viewModel);
        }

        [HttpGet]
        [Route("{controller}/{article}")]
        public async Task<IActionResult> Document(string article)
        {
            logService.LogMessage($"{DocumentActionName} has been called with: {article}");

            var careerPathSegmentModel = await jobProfileTasksSegmentService.GetByNameAsync(article).ConfigureAwait(false);

            if (careerPathSegmentModel != null)
            {
                var viewModel = mapper.Map<BodyViewModel>(careerPathSegmentModel);

                logService.LogMessage($"{DocumentActionName} has succeeded for: {article}");

                return View(nameof(Body), viewModel);
            }

            logService.LogMessage($"{DocumentActionName} has returned no content for: {article}");

            return NoContent();
        }

        [HttpGet]
        [Route("{controller}/{documentId}/contents")]
        public async Task<IActionResult> Body(Guid documentId)
        {
            logService.LogMessage($"{BodyActionName} has been called with: {documentId}");

            var model = await jobProfileTasksSegmentService.GetByIdAsync(documentId).ConfigureAwait(false);

            if (model != null)
            {
                var viewModel = mapper.Map<BodyViewModel>(model);

                var apiModel = mapper.Map<WhatYouWillDoApiModel>(model.Data);

                logService.LogMessage($"{BodyActionName} has succeeded for: {documentId}");

                return this.NegotiateContentResult(viewModel, apiModel);
            }

            logService.LogMessage($"{BodyActionName} has returned no content for: {documentId}", SeverityLevel.Warning);

            return NoContent();
        }

        [HttpPut]
        [Route("segment")]
        public async Task<IActionResult> Put([FromBody]JobProfileTasksSegmentModel jobProfileTasksSegmentModel)
        {
            logService.LogMessage($"{PutActionName} has been called");

            if (jobProfileTasksSegmentModel == null)
            {
                logService.LogMessage($"{PutActionName}. No document was passed");
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                logService.LogMessage($"{PutActionName}. Model state is invalid");
                return BadRequest(ModelState);
            }

            var existingDocument = await jobProfileTasksSegmentService.GetByIdAsync(jobProfileTasksSegmentModel.DocumentId).ConfigureAwait(false);
            if (existingDocument == null)
            {
                logService.LogMessage($"{PutActionName}. Couldnt find document with Id {jobProfileTasksSegmentModel.DocumentId}");
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }

            if (jobProfileTasksSegmentModel.SequenceNumber <= existingDocument.SequenceNumber)
            {
                logService.LogMessage($"{PutActionName}. Nothing to update as SequenceNumber of passed document {jobProfileTasksSegmentModel.SequenceNumber} is lower than SequenceNumber of persisted document {existingDocument.SequenceNumber}. ");
                return new StatusCodeResult((int)HttpStatusCode.AlreadyReported);
            }

            jobProfileTasksSegmentModel.Etag = existingDocument.Etag;
            jobProfileTasksSegmentModel.SocLevelTwo = existingDocument.SocLevelTwo;

            var response = await jobProfileTasksSegmentService.UpsertAsync(jobProfileTasksSegmentModel).ConfigureAwait(false);
            logService.LogMessage($"{PutActionName} has updated content for: {jobProfileTasksSegmentModel.CanonicalName}");

            return new StatusCodeResult((int)response);
        }

        [HttpPost]
        [Route("segment")]
        public async Task<IActionResult> Post([FromBody]JobProfileTasksSegmentModel jobProfileTasksSegmentModel)
        {
            logService.LogMessage($"{PostActionName} has been called");

            if (jobProfileTasksSegmentModel == null)
            {
                logService.LogMessage($"{PostActionName}. No document was passed");
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                logService.LogMessage($"{PostActionName}. Model state is invalid");
                return BadRequest(ModelState);
            }

            var existingDocument = await jobProfileTasksSegmentService.GetByIdAsync(jobProfileTasksSegmentModel.DocumentId).ConfigureAwait(false);
            if (existingDocument != null)
            {
                return new StatusCodeResult((int)HttpStatusCode.AlreadyReported);
            }

            var response = await jobProfileTasksSegmentService.UpsertAsync(jobProfileTasksSegmentModel).ConfigureAwait(false);

            logService.LogMessage($"{PostActionName} has created content for: {jobProfileTasksSegmentModel.CanonicalName}");

            return new StatusCodeResult((int)response);
        }

        [HttpDelete]
        [Route("{controller}/{documentId}")]
        public async Task<IActionResult> Delete(Guid documentId)
        {
            logService.LogMessage($"{DeleteActionName} has been called");

            var isDeleted = await jobProfileTasksSegmentService.DeleteAsync(documentId).ConfigureAwait(false);
            if (isDeleted)
            {
                logService.LogMessage($"{DeleteActionName} has deleted content for document Id: {documentId}");
                return Ok();
            }
            else
            {
                logService.LogMessage($"{DeleteActionName} has returned no content for: {documentId}", SeverityLevel.Warning);
                return NotFound();
            }
        }

        [HttpPatch]
        [Route("{controller}/{documentId}/uniform")]
        public async Task<IActionResult> PatchUniform([FromBody] PatchUniformModel patchDocument, Guid documentId)
        {
            logService.LogMessage($"{PatchUniformActionName} has been called");

            if (patchDocument == null)
            {
                logService.LogMessage($"{PatchUniformActionName}. No document was passed");
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                logService.LogMessage($"{PatchUniformActionName}. Model state is invalid");
                return BadRequest(ModelState);
            }

            var statusCode = await jobProfileTasksSegmentService.PatchUniformAsync(patchDocument, documentId).ConfigureAwait(false);

            return StatusCode((int)statusCode);
        }

        [HttpPatch]
        [Route("{controller}/{documentId}/location")]
        public async Task<IActionResult> PatchLocation([FromBody] PatchLocationModel patchDocument, Guid documentId)
        {
            logService.LogMessage($"{PatchLocationActionName} has been called");

            if (patchDocument == null)
            {
                logService.LogMessage($"{PatchLocationActionName}. No document was passed");
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                logService.LogMessage($"{PatchLocationActionName}. Model state is invalid");
                return BadRequest(ModelState);
            }

            var statusCode = await jobProfileTasksSegmentService.PatchLocationAsync(patchDocument, documentId).ConfigureAwait(false);

            return StatusCode((int)statusCode);
        }

        [HttpPatch]
        [Route("{controller}/{documentId}/environment")]
        public async Task<IActionResult> PatchEnvironment([FromBody] PatchEnvironmentsModel patchDocument, Guid documentId)
        {
            logService.LogMessage($"{PatchEnvironmentActionName} has been called");

            if (patchDocument == null)
            {
                logService.LogMessage($"{PatchEnvironmentActionName}. No document was passed");
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                logService.LogMessage($"{PatchEnvironmentActionName}. Model state is invalid");
                return BadRequest(ModelState);
            }

            var statusCode = await jobProfileTasksSegmentService.PatchEnvironmentAsync(patchDocument, documentId).ConfigureAwait(false);

            return StatusCode((int)statusCode);
        }
    }
}