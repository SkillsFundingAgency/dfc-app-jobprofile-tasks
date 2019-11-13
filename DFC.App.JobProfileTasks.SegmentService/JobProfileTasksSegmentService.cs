using AutoMapper;
using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using DFC.App.JobProfileTasks.Data.Models.ServiceBusModels;
using DFC.App.JobProfileTasks.Repository.CosmosDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

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

        public async Task<JobProfileTasksSegmentModel> GetByNameAsync(string canonicalName, bool isDraft = false)
        {
            if (string.IsNullOrWhiteSpace(canonicalName))
            {
                throw new ArgumentNullException(nameof(canonicalName));
            }

            return await repository.GetAsync(d => d.CanonicalName.ToLower() == canonicalName.ToLowerInvariant()).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> UpdateUniform(Guid documentId, JobProfileTasksDataUniformSegmentModel uniformSegmentModel)
        {
            if (uniformSegmentModel == null)
            {
                throw new ArgumentNullException(nameof(uniformSegmentModel));
            }

            var existingJobProfileEntity = await GetByIdAsync(documentId).ConfigureAwait(false);
            if (existingJobProfileEntity == null)
            {
                return HttpStatusCode.NotFound;
            }

            var matchingUniform = existingJobProfileEntity.Data.Uniforms.FirstOrDefault(x => x.Id == uniformSegmentModel.Id);
            if (matchingUniform == null)
            {
                return HttpStatusCode.NotFound;
            }

            var existingUniformItems = existingJobProfileEntity.Data.Uniforms.ToList();
            var existingItemIndex = existingUniformItems.FindIndex(x => x.Id == uniformSegmentModel.Id);

            existingUniformItems.RemoveAt(existingItemIndex);

            matchingUniform = mapper.Map<JobProfileTasksDataUniformSegmentModel>(uniformSegmentModel);
            existingUniformItems.Insert(existingItemIndex, matchingUniform);

            existingJobProfileEntity.Data.Uniforms = existingUniformItems;

            var result = await UpsertAsync(existingJobProfileEntity).ConfigureAwait(false);

            return result.ResponseStatusCode;
        }

        public async Task<HttpStatusCode> UpdateLocation(Guid documentId, JobProfileTasksDataLocationSegmentModel locationSegmentModel)
        {
            if (locationSegmentModel == null)
            {
                throw new ArgumentNullException(nameof(locationSegmentModel));
            }

            var existingJobProfileEntity = await GetByIdAsync(documentId).ConfigureAwait(false);
            if (existingJobProfileEntity == null)
            {
                return HttpStatusCode.NotFound;
            }

            var matchingLocation = existingJobProfileEntity.Data.Locations.FirstOrDefault(x => x.Id == locationSegmentModel.Id);
            if (matchingLocation == null)
            {
                return HttpStatusCode.NotFound;
            }

            var existingLocationItems = existingJobProfileEntity.Data.Locations.ToList();
            var existingItemIndex = existingLocationItems.FindIndex(x => x.Id == locationSegmentModel.Id);
            existingLocationItems.RemoveAt(existingItemIndex);
            existingLocationItems.Insert(existingItemIndex, locationSegmentModel);
            existingJobProfileEntity.Data.Locations = existingLocationItems;

            var result = await UpsertAsync(existingJobProfileEntity).ConfigureAwait(false);

            return result.ResponseStatusCode;
        }

        public async Task<HttpStatusCode> UpdateEnvironment(Guid documentId, JobProfileTasksDataEnvironmentSegmentModel environmentSegmentModel)
        {
            if (environmentSegmentModel == null)
            {
                throw new ArgumentNullException(nameof(environmentSegmentModel));
            }

            var existingJobProfileEntity = await GetByIdAsync(documentId).ConfigureAwait(false);
            if (existingJobProfileEntity == null)
            {
                return HttpStatusCode.NotFound;
            }

            var matchingEnvironment = existingJobProfileEntity.Data.Environments.FirstOrDefault(x => x.Id == environmentSegmentModel.Id);
            if (matchingEnvironment == null)
            {
                return HttpStatusCode.NotFound;
            }

            var existingEnvironmentItems = existingJobProfileEntity.Data.Environments.ToList();
            var existingItemIndex = existingEnvironmentItems.FindIndex(x => x.Id == environmentSegmentModel.Id);
            existingEnvironmentItems.RemoveAt(existingItemIndex);
            existingEnvironmentItems.Insert(existingItemIndex, environmentSegmentModel);
            existingJobProfileEntity.Data.Environments = existingEnvironmentItems;

            var result = await UpsertAsync(existingJobProfileEntity).ConfigureAwait(false);

            return result.ResponseStatusCode;
        }

        public async Task<HttpStatusCode> DeleteUniform(Guid jobProfileId, Guid uniformId)
        {
            var existingJobProfileEntity = await GetByIdAsync(jobProfileId).ConfigureAwait(false);
            if (existingJobProfileEntity == null)
            {
                return HttpStatusCode.NotFound;
            }

            var uniformToDelete = existingJobProfileEntity.Data.Uniforms.FirstOrDefault(x => x.Id == uniformId);
            if (uniformToDelete == null)
            {
                return HttpStatusCode.NotFound;
            }

            var uniforms = existingJobProfileEntity.Data.Uniforms.ToList();
            uniforms.Remove(uniformToDelete);
            existingJobProfileEntity.Data.Uniforms = uniforms;

            var result = await UpsertAsync(existingJobProfileEntity).ConfigureAwait(false);
            return result.ResponseStatusCode;
        }

        public async Task<HttpStatusCode> DeleteLocation(Guid jobProfileId, Guid locationId)
        {
            var existingJobProfileEntity = await GetByIdAsync(jobProfileId).ConfigureAwait(false);
            if (existingJobProfileEntity == null)
            {
                return HttpStatusCode.NotFound;
            }

            var itemToDelete = existingJobProfileEntity.Data.Locations.FirstOrDefault(x => x.Id == locationId);
            if (itemToDelete == null)
            {
                return HttpStatusCode.NotFound;
            }

            var locations = existingJobProfileEntity.Data.Locations.ToList();
            locations.Remove(itemToDelete);
            existingJobProfileEntity.Data.Locations = locations;

            var result = await UpsertAsync(existingJobProfileEntity).ConfigureAwait(false);
            return result.ResponseStatusCode;
        }

        public async Task<HttpStatusCode> DeleteEnvironment(Guid jobProfileId, Guid environmentId)
        {
            var existingJobProfileEntity = await GetByIdAsync(jobProfileId).ConfigureAwait(false);
            if (existingJobProfileEntity == null)
            {
                return HttpStatusCode.NotFound;
            }

            var itemToDelete = existingJobProfileEntity.Data.Environments.FirstOrDefault(x => x.Id == environmentId);
            if (itemToDelete == null)
            {
                return HttpStatusCode.NotFound;
            }

            var environments = existingJobProfileEntity.Data.Environments.ToList();
            environments.Remove(itemToDelete);
            existingJobProfileEntity.Data.Environments = environments;

            var result = await UpsertAsync(existingJobProfileEntity).ConfigureAwait(false);
            return result.ResponseStatusCode;
        }

        public async Task<UpsertJobProfileTasksModelResponse> UpsertAsync(JobProfileTasksSegmentModel tasksSegmentModel)
        {
            if (tasksSegmentModel == null)
            {
                throw new ArgumentNullException(nameof(tasksSegmentModel));
            }

            if (tasksSegmentModel.Data == null)
            {
                tasksSegmentModel.Data = new JobProfileTasksDataSegmentModel();
            }

            var result = await repository.UpsertAsync(tasksSegmentModel).ConfigureAwait(false);

            await UpsertAndRefreshSegmentModel(tasksSegmentModel).ConfigureAwait(false);

            return new UpsertJobProfileTasksModelResponse
            {
                JobProfileTasksSegmentModel = tasksSegmentModel,
                ResponseStatusCode = result,
            };
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