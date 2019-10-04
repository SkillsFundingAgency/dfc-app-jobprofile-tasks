using DFC.App.JobProfileTasks.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.Repository.CosmosDb
{
    public interface ICosmosRepository<T>
        where T : IDataModel
    {
        Task<bool> PingAsync();

        Task<T> GetAsync(Expression<Func<T, bool>> where);

        Task<IEnumerable<T>> GetAllAsync();

        Task<HttpStatusCode> UpsertAsync(T model);

        Task<HttpStatusCode> DeleteAsync(Guid documentId);
    }
}