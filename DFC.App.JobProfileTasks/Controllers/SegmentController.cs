using DFC.App.JobProfileTasks.ApiModels;
using DFC.App.JobProfileTasks.Data.Models.PatchModels;
using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using DFC.App.JobProfileTasks.Extensions;
using DFC.App.JobProfileTasks.SegmentService;
using DFC.App.JobProfileTasks.ViewModels;
using DFC.Logger.AppInsights.Contracts;
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
        [Route("{controller}")]
        public async Task<IActionResult> Index()
        {
            logService.LogInformation($"{IndexActionName} has been called");

            var viewModel = new IndexViewModel();
            var segmentModels = await jobProfileTasksSegmentService.GetAllAsync().ConfigureAwait(false);

            if (segmentModels != null)
            {
                viewModel.Documents = segmentModels
                    .OrderBy(x => x.CanonicalName)
                    .Select(x => mapper.Map<IndexDocumentViewModel>(x))
                    .ToList();

                logService.LogInformation($"{IndexActionName} has succeeded");
            }
            else
            {
                logService.LogWarning($"{IndexActionName} has returned with no results");
            }

            return View(viewModel);
        }

        [HttpGet]
        [Route("{controller}/{article}")]
        public async Task<IActionResult> Document(string article)
        {
            logService.LogInformation($"{DocumentActionName} has been called with: {article}");

            var careerPathSegmentModel = await jobProfileTasksSegmentService.GetByNameAsync(article).ConfigureAwait(false);

            if (careerPathSegmentModel != null)
            {
                var viewModel = mapper.Map<BodyViewModel>(careerPathSegmentModel);

                logService.LogInformation($"{DocumentActionName} has succeeded for: {article}");

                return View(nameof(Body), viewModel);
            }

            logService.LogInformation($"{DocumentActionName} has returned no content for: {article}");

            return NoContent();
        }

        [HttpGet]
        [Route("{controller}/{documentId}/contents")]
        public async Task<IActionResult> Body(Guid documentId)
        {
            logService.LogInformation($"{BodyActionName} has been called with: {documentId}");

            var model = await jobProfileTasksSegmentService.GetByIdAsync(documentId).ConfigureAwait(false);

            if (model != null)
            {
                var viewModel = mapper.Map<BodyViewModel>(model);

                var apiModel = mapper.Map<WhatYouWillDoApiModel>(model.Data);

                logService.LogInformation($"{BodyActionName} has succeeded for: {documentId}");

                return this.NegotiateContentResult(viewModel, apiModel);
            }

            logService.LogWarning($"{BodyActionName} has returned no content for: {documentId}");

            return NoContent();
        }

        [HttpPut]
        [Route("segment")]
        public async Task<IActionResult> Put([FromBody]JobProfileTasksSegmentModel jobProfileTasksSegmentModel)
        {
            logService.LogInformation($"{PutActionName} has been called");

            if (jobProfileTasksSegmentModel == null)
            {
                logService.LogInformation($"{PutActionName}. No document was passed");
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                logService.LogInformation($"{PutActionName}. Model state is invalid");
                return BadRequest(ModelState);
            }

            var existingDocument = await jobProfileTasksSegmentService.GetByIdAsync(jobProfileTasksSegmentModel.DocumentId).ConfigureAwait(false);
            if (existingDocument == null)
            {
                logService.LogInformation($"{PutActionName}. Couldnt find document with Id {jobProfileTasksSegmentModel.DocumentId}");
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }

            if (jobProfileTasksSegmentModel.SequenceNumber <= existingDocument.SequenceNumber)
            {
                logService.LogInformation($"{PutActionName}. Nothing to update as SequenceNumber of passed document {jobProfileTasksSegmentModel.SequenceNumber} is lower than SequenceNumber of persisted document {existingDocument.SequenceNumber}. ");
                return new StatusCodeResult((int)HttpStatusCode.AlreadyReported);
            }

            jobProfileTasksSegmentModel.Etag = existingDocument.Etag;
            jobProfileTasksSegmentModel.SocLevelTwo = existingDocument.SocLevelTwo;

            var response = await jobProfileTasksSegmentService.UpsertAsync(jobProfileTasksSegmentModel).ConfigureAwait(false);
            logService.LogInformation($"{PutActionName} has updated content for: {jobProfileTasksSegmentModel.CanonicalName}");

            return new StatusCodeResult((int)response);
        }

        [HttpPost]
        [Route("segment")]
        public async Task<IActionResult> Post([FromBody]JobProfileTasksSegmentModel jobProfileTasksSegmentModel)
        {
            logService.LogInformation($"{PostActionName} has been called");

            if (jobProfileTasksSegmentModel == null)
            {
                logService.LogInformation($"{PostActionName}. No document was passed");
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                logService.LogInformation($"{PostActionName}. Model state is invalid");
                return BadRequest(ModelState);
            }

            var existingDocument = await jobProfileTasksSegmentService.GetByIdAsync(jobProfileTasksSegmentModel.DocumentId).ConfigureAwait(false);
            if (existingDocument != null)
            {
                return new StatusCodeResult((int)HttpStatusCode.AlreadyReported);
            }

            var response = await jobProfileTasksSegmentService.UpsertAsync(jobProfileTasksSegmentModel).ConfigureAwait(false);

            logService.LogInformation($"{PostActionName} has created content for: {jobProfileTasksSegmentModel.CanonicalName}");

            return new StatusCodeResult((int)response);
        }

        [HttpDelete]
        [Route("{controller}/{documentId}")]
        public async Task<IActionResult> Delete(Guid documentId)
        {
            logService.LogInformation($"{DeleteActionName} has been called");

            var isDeleted = await jobProfileTasksSegmentService.DeleteAsync(documentId).ConfigureAwait(false);
            if (isDeleted)
            {
                logService.LogInformation($"{DeleteActionName} has deleted content for document Id: {documentId}");
                return Ok();
            }
            else
            {
                logService.LogWarning($"{DeleteActionName} has returned no content for: {documentId}");
                return NotFound();
            }
        }

        [HttpPatch]
        [Route("{controller}/{documentId}/uniform")]
        public async Task<IActionResult> PatchUniform([FromBody] PatchUniformModel patchDocument, Guid documentId)
        {
            logService.LogInformation($"{PatchUniformActionName} has been called");

            if (patchDocument == null)
            {
                logService.LogInformation($"{PatchUniformActionName}. No document was passed");
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                logService.LogInformation($"{PatchUniformActionName}. Model state is invalid");
                return BadRequest(ModelState);
            }

            var statusCode = await jobProfileTasksSegmentService.PatchUniformAsync(patchDocument, documentId).ConfigureAwait(false);

            return StatusCode((int)statusCode);
        }

        [HttpPatch]
        [Route("{controller}/{documentId}/location")]
        public async Task<IActionResult> PatchLocation([FromBody] PatchLocationModel patchDocument, Guid documentId)
        {
            logService.LogInformation($"{PatchLocationActionName} has been called");

            if (patchDocument == null)
            {
                logService.LogInformation($"{PatchLocationActionName}. No document was passed");
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                logService.LogInformation($"{PatchLocationActionName}. Model state is invalid");
                return BadRequest(ModelState);
            }

            var statusCode = await jobProfileTasksSegmentService.PatchLocationAsync(patchDocument, documentId).ConfigureAwait(false);

            return StatusCode((int)statusCode);
        }

        [HttpPatch]
        [Route("{controller}/{documentId}/environment")]
        public async Task<IActionResult> PatchEnvironment([FromBody] PatchEnvironmentsModel patchDocument, Guid documentId)
        {
            logService.LogInformation($"{PatchEnvironmentActionName} has been called");

            if (patchDocument == null)
            {
                logService.LogInformation($"{PatchEnvironmentActionName}. No document was passed");
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                logService.LogInformation($"{PatchEnvironmentActionName}. Model state is invalid");
                return BadRequest(ModelState);
            }

            var statusCode = await jobProfileTasksSegmentService.PatchEnvironmentAsync(patchDocument, documentId).ConfigureAwait(false);

            return StatusCode((int)statusCode);
        }
    }
}