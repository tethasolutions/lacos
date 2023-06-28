using System.Linq.Expressions;
using Lacos.GestioneCommesse.Domain;
using Microsoft.EntityFrameworkCore;

namespace Lacos.GestioneCommesse.Dal;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    IQueryable<TEntity> Query();

    Task<TEntity?> Get(long id);
    Task<TEntity?> Get(Expression<Func<TEntity, bool>> condition);

    Task Insert(TEntity entity);
    Task Insert(IEnumerable<TEntity> entities);

    void Update(TEntity entity);
    void Update(IEnumerable<TEntity> entities);
    Task Update(long id, Action<TEntity> updateAction);
    Task Update(Expression<Func<TEntity, bool>> condition, Action<TEntity> updateAction);

    Task InsertOrUpdate(TEntity entity);
    Task InsertOrUpdate(IEnumerable<TEntity> entities);

    Task Delete(long id);
    void Delete(TEntity entity);
    void Delete(IEnumerable<TEntity> entities);
    Task Delete(Expression<Func<TEntity, bool>> condition);
}

public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly DbSet<TEntity> dbSet;

    public Repository(ILacosDbContext dbContext)
    {
        dbSet = dbContext.Set<TEntity>();
    }

    public virtual IQueryable<TEntity> Query()
    {
        return dbSet;
    }

    public virtual Task<TEntity?> Get(long id)
    {
        return Get(e => e.Id == id);
    }

    public virtual Task<TEntity?> Get(Expression<Func<TEntity, bool>> condition)
    {
        return Query().FirstOrDefaultAsync(condition);
    }

    public virtual Task Insert(TEntity entity)
    {
        return dbSet.AddAsync(entity).AsTask();
    }

    public virtual Task Insert(IEnumerable<TEntity> entities)
    {
        return dbSet.AddRangeAsync(entities);
    }

    public virtual void Update(TEntity entity)
    {
        dbSet.Update(entity);
    }

    public virtual void Update(IEnumerable<TEntity> entities)
    {
        dbSet.UpdateRange(entities);
    }

    public virtual async Task Update(long id, Action<TEntity> updateAction)
    {
        var entity = await Get(id);

        if (entity != null)
        {
            updateAction(entity);

            Update(entity);
        }
    }

    public virtual async Task Update(Expression<Func<TEntity, bool>> condition, Action<TEntity> updateAction)
    {
        var entities = await Query()
            .Where(condition)
            .ToArrayAsync();

        foreach (var entity in entities)
        {
            updateAction(entity);

            Update(entity);
        }
    }

    public virtual Task InsertOrUpdate(TEntity entity)
    {
        if (entity.IsTransient())
        {
            return Insert(entity);
        }

        Update(entity);

        return Task.CompletedTask;
    }

    public virtual async Task InsertOrUpdate(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            await InsertOrUpdate(entity);
        }
    }

    public virtual async Task Delete(long id)
    {
        var entity = await Get(id);

        if (entity != null)
        {
            Delete(entity);
        }
    }

    public virtual void Delete(TEntity entity)
    {
        dbSet.Remove(entity);
    }

    public virtual void Delete(IEnumerable<TEntity> entities)
    {
        dbSet.RemoveRange(entities);
    }

    public virtual async Task Delete(Expression<Func<TEntity, bool>> condition)
    {
        var entities = await Query()
            .Where(condition)
            .ToArrayAsync();

        Delete(entities);
    }
}