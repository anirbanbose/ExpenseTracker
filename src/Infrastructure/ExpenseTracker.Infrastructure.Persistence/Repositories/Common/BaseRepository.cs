using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using ExpenseTracker.Domain.SharedKernel;
using System.Linq.Expressions;

namespace ExpenseTracker.Infrastructure.Persistence.Repositories.Common
{
    public abstract class BaseRepository<TEntity> where TEntity : class
    {
        public readonly ApplicationDbContext DbContext;
        protected DbSet<TEntity> Entities { get; }
        protected virtual IQueryable<TEntity> Table => Entities;
        protected virtual IQueryable<TEntity> TableNoTracking => Entities.AsNoTracking();

        protected BaseRepository(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
            Entities = DbContext.Set<TEntity>(); 
        }

        protected virtual async Task<TEntity?> GetSingle(Expression<Func<TEntity, bool>> searchExpression, CancellationToken cancellationToken)
        {
            return await Entities.FirstOrDefaultAsync(searchExpression, cancellationToken);
        }

        protected virtual async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> searchExpression, CancellationToken cancellationToken)
        {
            return await Entities.Where(searchExpression).ToListAsync(cancellationToken);
        }

        protected virtual async Task<List<TEntity>> ListAllAsync(CancellationToken cancellationToken)
        {
            return await Entities.ToListAsync(cancellationToken);
        }

        protected virtual async Task<List<TEntity>> ListAllNoTrackingAsync(CancellationToken cancellationToken)
        {
            return await TableNoTracking.ToListAsync(cancellationToken);
        }

        public async Task<PagedResult<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>> predicate, int pageIndex, int pageSize, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, string includeProperties, CancellationToken cancellationToken)
        {
            IQueryable<TEntity> query = TableNoTracking;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if(!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }            

            var totalCount = await query.CountAsync();

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            
            return PagedResult<TEntity>.SuccessResult(items, totalCount, pageIndex, pageSize);
        }

        protected virtual async Task AddAsync(TEntity entity)
        {
            await Entities.AddAsync(entity);
        }

        protected virtual void MarkAsUpdated(TEntity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            DbContext.Entry(entity!).State = EntityState.Modified;
        }

        protected virtual async Task UpdateAsync(
            Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> updateExpression)
        {
            await Entities.ExecuteUpdateAsync(updateExpression);
        }

        protected virtual async Task UpdateAsync(
            Expression<Func<TEntity, bool>> whereExpression, Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> updateExpression)
        {
            var entities = Entities.Where(whereExpression);
            if (entities.Any())
            {
                await entities.ExecuteUpdateAsync(updateExpression);
            }                
        }

        //protected virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> deleteExpression)
        //{
        //    var entities = Entities.Where(d => d is Entity).Where(deleteExpression).Select(x => x as Entity);
        //    if(entities != null && entities.Any())
        //    {
        //        foreach (var entity in entities)
        //        {
        //            entity!.MarkAsDelete();
        //            DbContext.Entry(entity).State = EntityState.Modified;
        //        }

        //        //await DbContext.SaveChangesAsync();
        //    }
            
        //}
    }
}
