using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Interfaces.Repositories
{
    public interface IAsyncRepository<TEntity>
    {
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, bool enableTracking = false, CancellationToken cancellationToken = default);

        Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, bool enableTracking = false, CancellationToken cancellationToken = default);

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate = null, bool enableTracking = false, CancellationToken cancellationToken = default);

        Task<TEntity> AddAsync(TEntity entity);

        Task<ICollection<TEntity>> AddRangeAsync(ICollection<TEntity> entities);

        Task<TEntity> UpdateAsync(TEntity entity);

        Task<ICollection<TEntity>> UpdateRangeAsync(ICollection<TEntity> entities);

        Task<TEntity> DeleteAsync(TEntity entity);

        Task<ICollection<TEntity>> DeleteRangeAsync(ICollection<TEntity> entities);
    }
}
