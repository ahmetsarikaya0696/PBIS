using Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Persistence.Contexts;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace Persistence.Repositories
{
    public class Repository<TEntity>
    : IAsyncRepository<TEntity> where TEntity : class
    {
        protected readonly ModelContext _context;
        public Repository(ModelContext context)
        {
            _context = context;
        }

        public IQueryable<TEntity> Query() => _context.Set<TEntity>();

        public async virtual Task<TEntity> AddAsync(TEntity entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<ICollection<TEntity>> AddRangeAsync(ICollection<TEntity> entities)
        {
            await _context.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
            return entities;
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate = null, bool enableTracking = false, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> queryable = Query();
            if (!enableTracking)
                queryable = queryable.AsNoTracking();
            return await queryable.AnyAsync(predicate, cancellationToken);
        }

        public async Task<TEntity> DeleteAsync(TEntity entity)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<ICollection<TEntity>> DeleteRangeAsync(ICollection<TEntity> entities)
        {
            _context.RemoveRange(entities);
            await _context.SaveChangesAsync();
            return entities;
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, bool enableTracking = false, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> queryable = Query();
            if (!enableTracking)
                queryable = queryable.AsNoTracking();
            if (include != null)
                queryable = include(queryable);
            return await queryable.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, bool enableTracking = false, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> queryable = Query();
            if (!enableTracking)
                queryable = queryable.AsNoTracking();
            if (include != null)
                queryable = include(queryable);
            if (predicate != null)
                queryable = queryable.Where(predicate);
            if (orderBy != null)
                return await orderBy(queryable).ToListAsync(cancellationToken);
            return await queryable.ToListAsync(cancellationToken);
        }

        public async virtual Task<TEntity> UpdateAsync(TEntity entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<ICollection<TEntity>> UpdateRangeAsync(ICollection<TEntity> entities)
        {
            _context.UpdateRange(entities);
            await _context.SaveChangesAsync();
            return entities;
        }
    }
}
