using DFC.App.JobProfileTasks.Data.Contracts;
using DFC.App.JobProfileTasks.Data.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.SegmentService
{
    public class JobProfileTasksSegmentService : IJobProfileTasksSegmentService
    {
        private readonly ICosmosRepository<JobProfileTasksSegmentModel> repository;
        private readonly IDraftJobProfileTasksSegmentService jobProfileTasksSegmentService;

        public JobProfileTasksSegmentService(
            ICosmosRepository<JobProfileTasksSegmentModel> repository,
            IDraftJobProfileTasksSegmentService jobProfileTasksSegmentService)
        {
            this.repository = repository;
            this.jobProfileTasksSegmentService = jobProfileTasksSegmentService;
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

        public async Task<JobProfileTasksSegmentModel> CreateAsync(JobProfileTasksSegmentModel overviewSegmentModel)
        {
            if (overviewSegmentModel == null)
            {
                throw new ArgumentNullException(nameof(overviewSegmentModel));
            }

            if (overviewSegmentModel.Data == null)
            {
                overviewSegmentModel.Data = new JobProfileTasksDataSegmentModel();
            }

            var result = await repository.CreateAsync(overviewSegmentModel).ConfigureAwait(false);

            return result == HttpStatusCode.Created
                ? await GetByIdAsync(overviewSegmentModel.DocumentId).ConfigureAwait(false)
                : null;
        }

        public async Task<JobProfileTasksSegmentModel> ReplaceAsync(JobProfileTasksSegmentModel overviewSegmentModel)
        {
            if (overviewSegmentModel == null)
            {
                throw new ArgumentNullException(nameof(overviewSegmentModel));
            }

            if (overviewSegmentModel.Data == null)
            {
                overviewSegmentModel.Data = new JobProfileTasksDataSegmentModel();
            }

            var result = await repository.UpdateAsync(overviewSegmentModel.DocumentId, overviewSegmentModel).ConfigureAwait(false);

            return result == HttpStatusCode.OK
                ? await GetByIdAsync(overviewSegmentModel.DocumentId).ConfigureAwait(false)
                : null;
        }

        public async Task<bool> DeleteAsync(Guid documentId, int partitionKeyValue)
        {
            var result = await repository.DeleteAsync(documentId, partitionKeyValue).ConfigureAwait(false);
            return result == HttpStatusCode.NoContent;
        }
    }
}
