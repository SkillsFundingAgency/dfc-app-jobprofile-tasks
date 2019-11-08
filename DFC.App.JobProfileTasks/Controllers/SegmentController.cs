﻿using DFC.App.JobProfileTasks.ApiModels;
using DFC.App.JobProfileTasks.Data.Models.PatchModels;
using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using DFC.App.JobProfileTasks.Extensions;
using DFC.App.JobProfileTasks.SegmentService;
using DFC.App.JobProfileTasks.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private const string DeleteUniformActionName = nameof(DeleteUniform);
        private const string DeleteLocationActionName = nameof(DeleteLocation);
        private const string DeleteEnvironmentActionName = nameof(DeleteEnvironment);

        private readonly ILogger<SegmentController> logger;
        private readonly IJobProfileTasksSegmentService jobProfileTasksSegmentService;
        private readonly AutoMapper.IMapper mapper;

        public SegmentController(ILogger<SegmentController> logger, IJobProfileTasksSegmentService jobProfileTasksSegmentService, AutoMapper.IMapper mapper)
        {
            this.logger = logger;
            this.jobProfileTasksSegmentService = jobProfileTasksSegmentService;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("/")]
        [Route("{controller}")]
        public async Task<IActionResult> Index()
        {
            logger.LogInformation($"{IndexActionName} has been called");

            var viewModel = new IndexViewModel();
            var segmentModels = await jobProfileTasksSegmentService.GetAllAsync().ConfigureAwait(false);

            if (segmentModels != null)
            {
                viewModel.Documents = segmentModels
                    .OrderBy(x => x.CanonicalName)
                    .Select(x => mapper.Map<IndexDocumentViewModel>(x))
                    .ToList();

                logger.LogInformation($"{IndexActionName} has succeeded");
            }
            else
            {
                logger.LogWarning($"{IndexActionName} has returned with no results");
            }

            return View(viewModel);
        }

        [HttpGet]
        [Route("{controller}/{article}")]
        public async Task<IActionResult> Document(string article)
        {
            logger.LogInformation($"{DocumentActionName} has been called with: {article}");

            var careerPathSegmentModel = await jobProfileTasksSegmentService.GetByNameAsync(article, Request.IsDraftRequest()).ConfigureAwait(false);

            if (careerPathSegmentModel != null)
            {
                var viewModel = mapper.Map<DocumentViewModel>(careerPathSegmentModel);

                logger.LogInformation($"{DocumentActionName} has succeeded for: {article}");

                return View(viewModel);
            }

            logger.LogWarning($"{DocumentActionName} has returned no content for: {article}");

            return NoContent();
        }

        [HttpGet]
        [Route("{controller}/{documentId}/contents")]
        public async Task<IActionResult> Body(Guid documentId)
        {
            logger.LogInformation($"{BodyActionName} has been called with: {documentId}");

            var model = await jobProfileTasksSegmentService.GetByIdAsync(documentId).ConfigureAwait(false);

            if (model != null)
            {
                var viewModel = mapper.Map<BodyViewModel>(model);

                var apiModel = mapper.Map<WhatYouWillDoApiModel>(model.Data);

                logger.LogInformation($"{BodyActionName} has succeeded for: {documentId}");

                return this.NegotiateContentResult(viewModel, apiModel);
            }

            logger.LogWarning($"{BodyActionName} has returned no content for: {documentId}");

            return NoContent();
        }

        [HttpPut]
        [Route("segment")]
        public async Task<IActionResult> Put([FromBody]JobProfileTasksSegmentModel upsertJobProfileTasksSegmentModel)
        {
            logger.LogInformation($"{PutActionName} has been called");

            if (upsertJobProfileTasksSegmentModel == null)
            {
                logger.LogInformation($"{PutActionName}. No document was passed");
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                logger.LogInformation($"{PutActionName}. Model state is invalid");
                return BadRequest(ModelState);
            }

            var existingDocument = await jobProfileTasksSegmentService.GetByIdAsync(upsertJobProfileTasksSegmentModel.DocumentId).ConfigureAwait(false);
            if (existingDocument == null)
            {
                logger.LogInformation($"{PutActionName}. Couldnt find document with Id {upsertJobProfileTasksSegmentModel.DocumentId}");
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }

            if (upsertJobProfileTasksSegmentModel.SequenceNumber <= existingDocument.SequenceNumber)
            {
                logger.LogInformation($"{PutActionName}. Nothing to update as SequenceNumber of passed document {upsertJobProfileTasksSegmentModel.SequenceNumber} is lower than SequenceNumber of persisted document {existingDocument.SequenceNumber}. ");
                return new StatusCodeResult((int)HttpStatusCode.AlreadyReported);
            }

            upsertJobProfileTasksSegmentModel.Etag = existingDocument.Etag;
            upsertJobProfileTasksSegmentModel.SocLevelTwo = existingDocument.SocLevelTwo;

            var response = await jobProfileTasksSegmentService.UpsertAsync(upsertJobProfileTasksSegmentModel).ConfigureAwait(false);
            logger.LogInformation($"{PutActionName} has updated content for: {upsertJobProfileTasksSegmentModel.CanonicalName}");

            return new OkObjectResult(response.JobProfileTasksSegmentModel);
        }

        [HttpPost]
        [Route("segment")]
        public async Task<IActionResult> Post([FromBody]JobProfileTasksSegmentModel upsertJobProfileTasksSegmentModel)
        {
            logger.LogInformation($"{PostActionName} has been called");

            if (upsertJobProfileTasksSegmentModel == null)
            {
                logger.LogInformation($"{PostActionName}. No document was passed");
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                logger.LogInformation($"{PostActionName}. Model state is invalid");
                return BadRequest(ModelState);
            }

            var existingDocument = await jobProfileTasksSegmentService.GetByIdAsync(upsertJobProfileTasksSegmentModel.DocumentId).ConfigureAwait(false);
            if (existingDocument != null)
            {
                return new StatusCodeResult((int)HttpStatusCode.AlreadyReported);
            }

            var response = await jobProfileTasksSegmentService.UpsertAsync(upsertJobProfileTasksSegmentModel).ConfigureAwait(false);

            logger.LogInformation($"{PostActionName} has created content for: {upsertJobProfileTasksSegmentModel.CanonicalName}");

            return new StatusCodeResult((int)response.ResponseStatusCode);
        }

        [HttpDelete]
        [Route("{controller}/{documentId}")]
        public async Task<IActionResult> Delete(Guid documentId)
        {
            logger.LogInformation($"{DeleteActionName} has been called");

            var isDeleted = await jobProfileTasksSegmentService.DeleteAsync(documentId).ConfigureAwait(false);
            if (isDeleted)
            {
                logger.LogInformation($"{DeleteActionName} has deleted content for document Id: {documentId}");
                return Ok();
            }
            else
            {
                logger.LogWarning($"{DeleteActionName} has returned no content for: {documentId}");
                return NotFound();
            }
        }

        [HttpPatch]
        [Route("{controller}/uniform")]
        public async Task<IActionResult> PatchUniform([FromBody] PatchUniformModel patchDocument)
        {
            logger.LogInformation($"{PatchUniformActionName} has been called");

            if (patchDocument == null)
            {
                logger.LogInformation($"{PatchUniformActionName}. No document was passed");
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                logger.LogInformation($"{PatchUniformActionName}. Model state is invalid");
                return BadRequest(ModelState);
            }

            var jobProfileTasksDataUniformSegmentModel = mapper.Map<JobProfileTasksDataUniformSegmentModel>(patchDocument);

            var statusCode = await jobProfileTasksSegmentService.UpdateUniform(patchDocument.JobProfileId, jobProfileTasksDataUniformSegmentModel).ConfigureAwait(false);

            return StatusCode((int)statusCode);
        }

        [HttpPatch]
        [Route("{controller}/location")]
        public async Task<IActionResult> PatchLocation([FromBody] PatchLocationModel patchDocument)
        {
            logger.LogInformation($"{PatchLocationActionName} has been called");

            if (patchDocument == null)
            {
                logger.LogInformation($"{PatchLocationActionName}. No document was passed");
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                logger.LogInformation($"{PatchLocationActionName}. Model state is invalid");
                return BadRequest(ModelState);
            }

            var jobProfileTasksDataLocationSegmentModel = mapper.Map<JobProfileTasksDataLocationSegmentModel>(patchDocument);

            var statusCode = await jobProfileTasksSegmentService.UpdateLocation(patchDocument.JobProfileId, jobProfileTasksDataLocationSegmentModel).ConfigureAwait(false);

            return StatusCode((int)statusCode);
        }

        [HttpPatch]
        [Route("{controller}/environment")]
        public async Task<IActionResult> PatchEnvironment([FromBody] PatchEnvironmentsModel patchDocument)
        {
            logger.LogInformation($"{PatchEnvironmentActionName} has been called");

            if (patchDocument == null)
            {
                logger.LogInformation($"{PatchEnvironmentActionName}. No document was passed");
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                logger.LogInformation($"{PatchEnvironmentActionName}. Model state is invalid");
                return BadRequest(ModelState);
            }

            var jobProfileTasksDataEnvironmentSegmentModel = mapper.Map<JobProfileTasksDataEnvironmentSegmentModel>(patchDocument);

            var statusCode = await jobProfileTasksSegmentService.UpdateEnvironment(patchDocument.JobProfileId, jobProfileTasksDataEnvironmentSegmentModel).ConfigureAwait(false);

            return StatusCode((int)statusCode);
        }

        [HttpPatch]
        [Route("{controller}/{jobProfileId}/uniform/{id}")]
        public async Task<IActionResult> DeleteUniform(DeleteUniformModel deleteUniformModel)
        {
            logger.LogInformation($"{DeleteUniformActionName} has been called");

            if (deleteUniformModel == null)
            {
                logger.LogInformation($"{DeleteUniformActionName}. No document was passed");
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                logger.LogInformation($"{DeleteUniformActionName}. Model state is invalid");
                return BadRequest(ModelState);
            }

            var result = await jobProfileTasksSegmentService.DeleteUniform(deleteUniformModel.JobProfileId, deleteUniformModel.Id).ConfigureAwait(false);

            return StatusCode((int)result);
        }

        [HttpPatch]
        [Route("{controller}/{jobProfileId}/location/{id}")]
        public async Task<IActionResult> DeleteLocation(DeleteLocationModel deleteLocationModel)
        {
            logger.LogInformation($"{DeleteLocationActionName} has been called");

            if (deleteLocationModel == null)
            {
                logger.LogInformation($"{DeleteLocationActionName}. No document was passed");
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                logger.LogInformation($"{DeleteLocationActionName}. Model state is invalid");
                return BadRequest(ModelState);
            }

            var result = await jobProfileTasksSegmentService.DeleteLocation(deleteLocationModel.JobProfileId, deleteLocationModel.Id).ConfigureAwait(false);

            return StatusCode((int)result);
        }

        [HttpPatch]
        [Route("{controller}/{jobProfileId}/environment/{id}")]
        public async Task<IActionResult> DeleteEnvironment(DeleteEnvironmentModel deleteEnvironmentModel)
        {
            logger.LogInformation($"{DeleteEnvironmentActionName} has been called");

            if (deleteEnvironmentModel == null)
            {
                logger.LogInformation($"{DeleteEnvironmentActionName}. No document was passed");
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                logger.LogInformation($"{DeleteEnvironmentActionName}. Model state is invalid");
                return BadRequest(ModelState);
            }

            var result = await jobProfileTasksSegmentService.DeleteEnvironment(deleteEnvironmentModel.JobProfileId, deleteEnvironmentModel.Id).ConfigureAwait(false);

            return StatusCode((int)result);
        }
    }
}