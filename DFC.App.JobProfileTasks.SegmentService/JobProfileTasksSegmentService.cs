using AutoMapper;
using DFC.App.JobProfileTasks.Data.Enums;
using DFC.App.JobProfileTasks.Data.Models.PatchModels;
using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using DFC.App.JobProfileTasks.Data.Models.ServiceBusModels;
using DFC.App.JobProfileTasks.Repository.CosmosDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Environment = DFC.App.JobProfileTasks.Data.Models.SegmentModels.Environment;
using Location = DFC.App.JobProfileTasks.Data.Models.SegmentModels.Location;
using Uniform = DFC.App.JobProfileTasks.Data.Models.SegmentModels.Uniform;

namespace DFC.App.JobProfileTasks.SegmentService
{
    public class JobProfileTasksSegmentService : IJobProfileTasksSegmentService
    {
        private readonly ICosmosRepository<JobProfileTasksSegmentModel> repository;
        private readonly IMapper mapper;
        private readonly IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> jobProfileSegmentRefreshService;

        public JobProfileTasksSegmentService(
            ICosmosRepository<JobProfileTasksSegmentModel> repository,
            IMapper mapper,
            IJobProfileSegmentRefreshService<RefreshJobProfileSegmentServiceBusModel> jobProfileSegmentRefreshService)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.jobProfileSegmentRefreshService = jobProfileSegmentRefreshService;
        }

        public async Task<bool> PingAsync()
        {
            return await repository.PingAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<JobProfileTasksSegmentModel>> GetAllAsync()
        {
            return await repository.GetAllAsync().ConfigureAwait(false);
        }

        public async Task<JobProfileTasksSegmentModel> GetByIdAsync(Guid documentId)
        {
            return await repository.GetAsync(d => d.DocumentId == documentId).ConfigureAwait(false);
        }

        public async Task<JobProfileTasksSegmentModel> GetByNameAsync(string canonicalName)
        {
            if (string.IsNullOrWhiteSpace(canonicalName))
            {
                throw new ArgumentNullException(nameof(canonicalName));
            }

            return await repository.GetAsync(d => d.CanonicalName.ToLower() == canonicalName.ToLowerInvariant()).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> PatchUniformAsync(PatchUniformModel patchModel, Guid documentId)
        {
            if (patchModel == null)
            {
                throw new ArgumentNullException(nameof(patchModel));
            }

            var existingSegmentModel = await GetByIdAsync(documentId).ConfigureAwait(false);
            if (existingSegmentModel is null)
            {
                return HttpStatusCode.NotFound;
            }

            if (patchModel.SequenceNumber <= existingSegmentModel.SequenceNumber)
            {
                return HttpStatusCode.AlreadyReported;
            }

            var existingUniform = existingSegmentModel.Data?.Uniforms?.FirstOrDefault(u => u.Id == patchModel.Id);
            if (existingUniform is null)
            {
                return patchModel.MessageAction == MessageActionType.Deleted ? HttpStatusCode.AlreadyReported : HttpStatusCode.NotFound;
            }

            var existingIndex = existingSegmentModel.Data.Uniforms.ToList().FindIndex(ai => ai.Id == patchModel.Id);
            if (patchModel.MessageAction == MessageActionType.Deleted)
            {
                existingSegmentModel.Data.Uniforms.RemoveAt(existingIndex);
            }
            else
            {
                var updatedUniform = mapper.Map<Uniform>(patchModel);
                existingSegmentModel.Data.Uniforms[existingIndex] = updatedUniform;
            }

            existingSegmentModel.SequenceNumber = patchModel.SequenceNumber;

            return await UpsertAndRefreshSegmentModel(existingSegmentModel).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> PatchLocationAsync(PatchLocationModel patchModel, Guid documentId)
        {
            if (patchModel == null)
            {
                throw new ArgumentNullException(nameof(patchModel));
            }

            var existingSegmentModel = await GetByIdAsync(documentId).ConfigureAwait(false);
            if (existingSegmentModel is null)
            {
                return HttpStatusCode.NotFound;
            }

            if (patchModel.SequenceNumber <= existingSegmentModel.SequenceNumber)
            {
                return HttpStatusCode.AlreadyReported;
            }

            var existingLocation = existingSegmentModel.Data?.Locations?.FirstOrDefault(u => u.Id == patchModel.Id);
            if (existingLocation is null)
            {
                return patchModel.MessageAction == MessageActionType.Deleted ? HttpStatusCode.AlreadyReported : HttpStatusCode.NotFound;
            }

            var existingIndex = existingSegmentModel.Data.Locations.ToList().FindIndex(ai => ai.Id == patchModel.Id);
            if (patchModel.MessageAction == MessageActionType.Deleted)
            {
                existingSegmentModel.Data.Locations.RemoveAt(existingIndex);
            }
            else
            {
                var updatedLocation = mapper.Map<Location>(patchModel);
                existingSegmentModel.Data.Locations[existingIndex] = updatedLocation;
            }

            existingSegmentModel.SequenceNumber = patchModel.SequenceNumber;

            return await UpsertAndRefreshSegmentModel(existingSegmentModel).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> PatchEnvironmentAsync(PatchEnvironmentsModel patchModel, Guid documentId)
        {
            if (patchModel == null)
            {
                throw new ArgumentNullException(nameof(patchModel));
            }

            var existingSegmentModel = await GetByIdAsync(documentId).ConfigureAwait(false);
            if (existingSegmentModel is null)
            {
                return HttpStatusCode.NotFound;
            }

            if (patchModel.SequenceNumber <= existingSegmentModel.SequenceNumber)
            {
                return HttpStatusCode.AlreadyReported;
            }

            var existingEnvironment = existingSegmentModel.Data?.Environments?.FirstOrDefault(u => u.Id == patchModel.Id);
            if (existingEnvironment is null)
            {
                return patchModel.MessageAction == MessageActionType.Deleted ? HttpStatusCode.AlreadyReported : HttpStatusCode.NotFound;
            }

            var existingIndex = existingSegmentModel.Data.Environments.ToList().FindIndex(ai => ai.Id == patchModel.Id);
            if (patchModel.MessageAction == MessageActionType.Deleted)
            {
                existingSegmentModel.Data.Environments.RemoveAt(existingIndex);
            }
            else
            {
                var updatedEnvironment = mapper.Map<Environment>(patchModel);
                existingSegmentModel.Data.Environments[existingIndex] = updatedEnvironment;
            }

            existingSegmentModel.SequenceNumber = patchModel.SequenceNumber;

            return await UpsertAndRefreshSegmentModel(existingSegmentModel).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> UpsertAsync(JobProfileTasksSegmentModel tasksSegmentModel)
        {
            if (tasksSegmentModel == null)
            {
                throw new ArgumentNullException(nameof(tasksSegmentModel));
            }

            if (tasksSegmentModel.Data == null)
            {
                tasksSegmentModel.Data = new JobProfileTasksDataSegmentModel();
            }

            return await UpsertAndRefreshSegmentModel(tasksSegmentModel).ConfigureAwait(false);
        }

        public async Task<bool> DeleteAsync(Guid documentId)
        {
            var result = await repository.DeleteAsync(documentId).ConfigureAwait(false);
            return result == HttpStatusCode.NoContent;
        }

        private async Task<HttpStatusCode> UpsertAndRefreshSegmentModel(JobProfileTasksSegmentModel existingSegmentModel)
        {
            var result = await repository.UpsertAsync(existingSegmentModel).ConfigureAwait(false);

            if (result == HttpStatusCode.OK || result == HttpStatusCode.Created)
            {
                var refreshJobProfileSegmentServiceBusModel = mapper.Map<RefreshJobProfileSegmentServiceBusModel>(existingSegmentModel);

                await jobProfileSegmentRefreshService.SendMessageAsync(refreshJobProfileSegmentServiceBusModel).ConfigureAwait(false);
            }

            return result;
        }
    }
}