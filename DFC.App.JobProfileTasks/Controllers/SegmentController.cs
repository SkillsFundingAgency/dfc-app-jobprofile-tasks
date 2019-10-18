using DFC.App.JobProfileTasks.Data.Models;
using DFC.App.JobProfileTasks.Extensions;
using DFC.App.JobProfileTasks.SegmentService;
using DFC.App.JobProfileTasks.ViewModels;
using Microsoft.AspNetCore.JsonPatch;
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
        private const string EnvironmentsLeadingText = "Your working environment may be";
        private const string LocationLeadingText = "You could work";
        private const string UniformLeadingText = "You may need to wear";
        private const string EnvironmentsConjunction = "and";
        private const string LocationConjunction = "or";
        private const string UniformConjunction = "and";
        private const string IndexActionName = nameof(Index);
        private const string DocumentActionName = nameof(Document);
        private const string BodyActionName = nameof(Body);
        private const string SaveActionName = nameof(Save);
        private const string PutActionName = nameof(Put);
        private const string PostActionName = nameof(Post);
        private const string DeleteActionName = nameof(Delete);
        private const string PatchActionName = nameof(Patch);

        private readonly ILogger<SegmentController> logger;
        private readonly IJobProfileTasksSegmentService jobProfileTasksSegmentService;
        private readonly AutoMapper.IMapper mapper;
        private readonly IFormatContentService formatContentService;

        public SegmentController(ILogger<SegmentController> logger, IJobProfileTasksSegmentService jobProfileTasksSegmentService, AutoMapper.IMapper mapper, IFormatContentService formatContentService)
        {
            this.logger = logger;
            this.jobProfileTasksSegmentService = jobProfileTasksSegmentService;
            this.mapper = mapper;
            this.formatContentService = formatContentService;
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
        [Route("{controller}/{article}/contents")]
        public async Task<IActionResult> Body(string article)
        {
            logger.LogInformation($"{BodyActionName} has been called with: {article}");

            var model = await jobProfileTasksSegmentService.GetByNameAsync(article, Request.IsDraftRequest()).ConfigureAwait(false);

            if (model != null)
            {
                var viewModel = mapper.Map<BodyViewModel>(model);

                if (model.Data != null)
                {
                    viewModel.Data.Environment = formatContentService.GetParagraphText(EnvironmentsLeadingText, model.Data?.Environment?.Select(x => x.Description), EnvironmentsConjunction);
                    viewModel.Data.Location = formatContentService.GetParagraphText(LocationLeadingText, model.Data?.Location?.Select(x => x.Description), LocationConjunction);
                    viewModel.Data.Uniform = formatContentService.GetParagraphText(UniformLeadingText, model.Data?.Uniform?.Select(x => x.Description), UniformConjunction);
                }

                logger.LogInformation($"{BodyActionName} has succeeded for: {article}");

                return this.NegotiateContentResult(viewModel, model.Data);
            }

            logger.LogWarning($"{BodyActionName} has returned no content for: {article}");

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

            var response = await jobProfileTasksSegmentService.UpsertAsync(upsertJobProfileTasksSegmentModel).ConfigureAwait(false);

            logger.LogInformation($"{PostActionName} has created content for: {upsertJobProfileTasksSegmentModel.CanonicalName}");

            return new CreatedAtActionResult(
                PostActionName,
                "Segment",
                new { article = response.JobProfileTasksSegmentModel.CanonicalName },
                response.JobProfileTasksSegmentModel);
        }

        public async Task<IActionResult> Save([FromBody]JobProfileTasksSegmentModel upsertJobProfileTasksSegmentModel)
        {
            logger.LogInformation($"{SaveActionName} has been called");

            if (upsertJobProfileTasksSegmentModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await jobProfileTasksSegmentService.UpsertAsync(upsertJobProfileTasksSegmentModel).ConfigureAwait(false);

            if (response.ResponseStatusCode == HttpStatusCode.Created)
            {
                logger.LogInformation($"{SaveActionName} has created content for: {upsertJobProfileTasksSegmentModel.CanonicalName}");

                return new CreatedAtActionResult(
                    SaveActionName,
                    "Segment",
                    new { article = response.JobProfileTasksSegmentModel.CanonicalName },
                    response.JobProfileTasksSegmentModel);
            }
            else
            {
                logger.LogInformation($"{SaveActionName} has updated content for: {upsertJobProfileTasksSegmentModel.CanonicalName}");

                return new OkObjectResult(response.JobProfileTasksSegmentModel);
            }
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
        [Route("{controller}/{article}")]
        public async Task<IActionResult> Patch([FromBody] JsonPatchDocument<JobProfileTasksSegmentModel> patchDocument, string article)
        {
            logger.LogInformation($"{PatchActionName} has been called");

            if (patchDocument == null)
            {
                return BadRequest();
            }

            var model = await jobProfileTasksSegmentService.GetByNameAsync(article, Request.IsDraftRequest()).ConfigureAwait(false);
            if (model == null)
            {
                logger.LogWarning($"{PatchActionName} has returned no content for: {article}");
                return NotFound();
            }

            patchDocument.ApplyTo(model);

            var viewModel = mapper.Map<BodyViewModel>(model);
            return Ok(viewModel);
        }
    }
}