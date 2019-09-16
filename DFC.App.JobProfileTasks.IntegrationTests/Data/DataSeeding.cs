using DFC.App.JobProfileTasks.Data.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.IntegrationTests.Data
{
    public class DataSeeding
    {
        private const string Segment = "segment";

        public DataSeeding()
        {
            Article1Id = Guid.NewGuid();
            Article2Id = Guid.NewGuid();
            Article3Id = Guid.NewGuid();

            Article1Name = Article1Id.ToString();
            Article2Name = Article2Id.ToString();
            Article3Name = Article3Id.ToString();

            Created = DateTime.UtcNow;
        }

        public Guid Article1Id { get; private set; }

        public Guid Article2Id { get; private set; }

        public Guid Article3Id { get; private set; }

        public string Article1Name { get; private set; }

        public string Article2Name { get; private set; }

        public string Article3Name { get; private set; }

        public DateTime Created { get; private set; }

        public async Task AddData(CustomWebApplicationFactory<Startup> factory)
        {
            var url = $"/{Segment}";
            var models = CreateModels();

            var client = factory?.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            foreach (var model in models)
            {
                await client.PostAsync(url, model, new JsonMediaTypeFormatter()).ConfigureAwait(false);
            }
        }

        public async Task RemoveData(CustomWebApplicationFactory<Startup> factory)
        {
            var models = CreateModels();

            var client = factory?.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            foreach (var model in models)
            {
                var url = string.Concat("/", Segment, "/", model.DocumentId);
                await client.DeleteAsync(url).ConfigureAwait(false);
            }
        }

        private List<JobProfileTasksSegmentModel> CreateModels()
        {
            var models = new List<JobProfileTasksSegmentModel>()
            {
                new JobProfileTasksSegmentModel()
                {
                    Created = Created,
                    DocumentId = Article1Id,
                    CanonicalName = Article1Name,
                },
                new JobProfileTasksSegmentModel()
                {
                    Created = Created,
                    DocumentId = Article2Id,
                    CanonicalName = Article2Name,
                },
                new JobProfileTasksSegmentModel()
                {
                    Created = Created,
                    DocumentId = Article3Id,
                    CanonicalName = Article3Name,
                },
            };

            return models;
        }
    }
}