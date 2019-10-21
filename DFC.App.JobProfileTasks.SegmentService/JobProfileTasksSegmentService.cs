using AutoMapper;
using DFC.App.JobProfileTasks.Data.Models;
using DFC.App.JobProfileTasks.Data.Models.PatchModels;
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

        public JobProfileTasksSegmentService(ICosmosRepository<JobProfileTasksSegmentModel> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
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

        public async Task<HttpStatusCode> Update(PatchUniformModel patchUniform)
        {
            if (patchUniform == null)
            {
                throw new ArgumentNullException(nameof(patchUniform));
            }

            var existingJobProfileEntity = await GetByIdAsync(patchUniform.JobProfileId).ConfigureAwait(false);
            if (existingJobProfileEntity == null)
            {
                return HttpStatusCode.NotFound;
            }

            var matchingUniform = existingJobProfileEntity.Data.Uniforms.FirstOrDefault(x => x.Id == patchUniform.UniformId);
            if (matchingUniform == null)
            {
                return HttpStatusCode.NotFound;
            }

            var existingUniformItems = existingJobProfileEntity.Data.Uniforms.ToList();
            var existingItemIndex = existingUniformItems.FindIndex(x => x.Id == patchUniform.UniformId);

            existingUniformItems.RemoveAt(existingItemIndex);

            matchingUniform = mapper.Map<JobProfileTasksDataUniformSegmentModel>(patchUniform);
            existingUniformItems.Insert(existingItemIndex, matchingUniform);

            existingJobProfileEntity.Data.Uniforms = existingUniformItems;

            var result = await UpsertAsync(existingJobProfileEntity).ConfigureAwait(false);

            return result.ResponseStatusCode;
        }

        public async Task<HttpStatusCode> Delete(Guid jobProfileId, Guid uniformId)
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
    }
}