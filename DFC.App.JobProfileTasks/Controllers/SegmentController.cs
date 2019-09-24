using DFC.App.JobProfileTasks.Data.Contracts;
using DFC.App.JobProfileTasks.Data.Models;
using DFC.App.JobProfileTasks.Extensions;
using DFC.App.JobProfileTasks.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
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
            logger.LogInformation($"{nameof(Index)} has been called");

            var viewModel = new IndexViewModel();
            var segmentModels = await jobProfileTasksSegmentService.GetAllAsync().ConfigureAwait(false);

            if (segmentModels != null)
            {
                viewModel.Documents = segmentModels
                    .OrderBy(x => x.CanonicalName)
                    .Select(x => mapper.Map<IndexDocumentViewModel>(x))
                    .ToList();

                logger.LogInformation($"{nameof(Index)} has succeeded");
            }
            else
            {
                logger.LogWarning($"{nameof(Index)} has returned with no results");
            }

            return View(viewModel);
        }

        [HttpGet]
        [Route("{controller}/{article}")]
        public async Task<IActionResult> Document(string article)
        {
            logger.LogInformation($"{nameof(Document)} has been called with: {article}");

            var careerPathSegmentModel = await jobProfileTasksSegmentService.GetByNameAsync(article, Request.IsDraftRequest()).ConfigureAwait(false);

            if (careerPathSegmentModel != null)
            {
                var viewModel = mapper.Map<DocumentViewModel>(careerPathSegmentModel);

                logger.LogInformation($"{nameof(Document)} has succeeded for: {article}");

                return View(viewModel);
            }

            logger.LogWarning($"{nameof(Document)} has returned no content for: {article}");

            return NoContent();
        }

        [HttpGet]
        [Route("{controller}/{article}/contents")]
        public async Task<IActionResult> Body(string article)
        {
            logger.LogInformation($"{nameof(Body)} has been called with: {article}");

            var model = await jobProfileTasksSegmentService.GetByNameAsync(article, Request.IsDraftRequest()).ConfigureAwait(false);

            if (model != null)
            {
                var viewModel = mapper.Map<BodyViewModel>(model);

                if (model.Data != null)
                {
                    viewModel.Data.Environment = formatContentService.GetParagraphText(EnvironmentsLeadingText, model.Data.Environments.Select(x => x.Description), EnvironmentsConjunction);
                    viewModel.Data.Location = formatContentService.GetParagraphText(LocationLeadingText, model.Data.Locations.Select(x => x.Description), LocationConjunction);
                    viewModel.Data.Uniform = formatContentService.GetParagraphText(UniformLeadingText, model.Data.Uniforms.Select(x => x.Description), UniformConjunction);
                }

                logger.LogInformation($"{nameof(Body)} has succeeded for: {article}");

                return this.NegotiateContentResult(viewModel, model.Data);
            }

            logger.LogWarning($"{nameof(Body)} has returned no content for: {article}");

            return NoContent();
        }

        [HttpPut]
        [HttpPost]
        [Route("{controller}")]
        public async Task<IActionResult> CreateOrUpdate([FromBody]JobProfileTasksSegmentModel createOrUpdateSegmentModel)
        {
            logger.LogInformation($"{nameof(CreateOrUpdate)} has been called");

            if (createOrUpdateSegmentModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingCareerPathSegmentModel = await jobProfileTasksSegmentService.GetByIdAsync(createOrUpdateSegmentModel.DocumentId).ConfigureAwait(false);

            if (existingCareerPathSegmentModel == null)
            {
                var createdResponse = await jobProfileTasksSegmentService.CreateAsync(createOrUpdateSegmentModel).ConfigureAwait(false);

                logger.LogInformation($"{nameof(CreateOrUpdate)} has created content for: {createOrUpdateSegmentModel.CanonicalName}");

                return new CreatedAtActionResult(nameof(Document), "Segment", new { article = createdResponse.CanonicalName }, createdResponse);
            }
            else
            {
                var updatedResponse = await jobProfileTasksSegmentService.ReplaceAsync(createOrUpdateSegmentModel).ConfigureAwait(false);

                logger.LogInformation($"{nameof(CreateOrUpdate)} has updated content for: {createOrUpdateSegmentModel.CanonicalName}");

                return new OkObjectResult(updatedResponse);
            }
        }

        [HttpDelete]
        [Route("{controller}/{documentId}")]
        public async Task<IActionResult> Delete(Guid documentId)
        {
            logger.LogInformation($"{nameof(Delete)} has been called");

            var jobProfileTasksSegmentModel = await jobProfileTasksSegmentService.GetByIdAsync(documentId).ConfigureAwait(false);

            if (jobProfileTasksSegmentModel == null)
            {
                logger.LogWarning($"{nameof(Document)} has returned no content for: {documentId}");

                return NotFound();
            }

            await jobProfileTasksSegmentService.DeleteAsync(documentId, jobProfileTasksSegmentModel.PartitionKey).ConfigureAwait(false);

            logger.LogInformation($"{nameof(Delete)} has deleted content for: {jobProfileTasksSegmentModel.CanonicalName}");

            return Ok();
        }
    }
}
