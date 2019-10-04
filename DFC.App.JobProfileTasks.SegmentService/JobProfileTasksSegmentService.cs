using DFC.App.JobProfileTasks.Data.Models;
using DFC.App.JobProfileTasks.Repository.CosmosDb;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.SegmentService
{
    public class JobProfileTasksSegmentService : IJobProfileTasksSegmentService
    {
        private readonly ICosmosRepository<JobProfileTasksSegmentModel> repository;

        public JobProfileTasksSegmentService(ICosmosRepository<JobProfileTasksSegmentModel> repository)
        {
            this.repository = repository;
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