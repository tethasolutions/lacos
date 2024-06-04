using System.Linq.Expressions;
using Lacos.GestioneCommesse.Dal;
using Microsoft.EntityFrameworkCore;

namespace Westcar.WebApplication.Dal;

public interface IViewRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> Query();

    Task<TEntity?> Get(Expression<Func<TEntity, bool>> condition);

}

public class ViewRepository<TEntity> : IViewRepository<TEntity> where TEntity : class
{
    protected readonly DbSet<TEntity> dbSet;

    public ViewRepository(ILacosDbContext dbContext)
    {
        dbSet = dbContext.Set<TEntity>();
    }

    public virtual IQueryable<TEntity> Query()
    {
        return dbSet;
    }


    public virtual Task<TEntity?> Get(Expression<Func<TEntity, bool>> condition)
    {
        return Query().FirstOrDefaultAsync(condition);
    }

}