using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Lacos.GestioneCommesse.Domain;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Session;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Lacos.GestioneCommesse.Dal;

public interface ILacosDbContext
{
    Task SaveChanges();
    void RejectChanges();
    Task<IDbContextTransaction> BeginTransaction();
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task ExecuteWithEnabledQueryFilters(Func<Task> action, params QueryFilter[] queryFiltersEnabled);
    Task<T> ExecuteWithEnabledQueryFilters<T>(Func<Task<T>> func, params QueryFilter[] queryFiltersEnabled);
    Task ExecuteWithDisabledQueryFilters(Func<Task> action, params QueryFilter[] queryFiltersDisabled);
    Task<T> ExecuteWithDisabledQueryFilters<T>(Func<Task<T>> func, params QueryFilter[] queryFiltersDisabled);
    IQueryable<T> ExecuteStoredProcedure<T>(string storedProcedureName, params SqlParameter[] parameters) where T : class;
    IQueryable<T> ExecuteStoredProcedure<T>(string storedProcedureName) where T : class;
}

public enum QueryFilter
{
    SoftDelete,
    OperatorEntity
}

public class LacosDbContext : DbContext, ILacosDbContext
{
    private readonly ILacosSession session;

    private IDictionary<QueryFilter, bool> QueryFilters { get; set; }

    public LacosDbContext(
        DbContextOptions options,
        ILacosSession session
    )
        : base(options)
    {
        this.session = session;

        QueryFilters = GetDefaultQueryFilters(true);
    }

    public Task<IDbContextTransaction> BeginTransaction()
    {
        return Database.CurrentTransaction == null
            ? Database.BeginTransactionAsync(IsolationLevel.Unspecified)
            : Task.FromResult(Database.CurrentTransaction);
    }

    public new DbSet<TEntity> Set<TEntity>() where TEntity : class
    {
        return base.Set<TEntity>();
    }

    public Task ExecuteWithEnabledQueryFilters(Func<Task> action, params QueryFilter[] queryFiltersEnabled)
    {
        var currentQueryFilters = QueryFilters;
        QueryFilters = GetDefaultQueryFilters(false)
            .ToDictionary(e => e.Key, e => queryFiltersEnabled.Contains(e.Key));

        try
        {
            return action();
        }
        finally
        {
            QueryFilters = currentQueryFilters;
        }
    }

    public Task<T> ExecuteWithEnabledQueryFilters<T>(Func<Task<T>> func,
        params QueryFilter[] queryFiltersEnabled)
    {
        var currentQueryFilters = QueryFilters;
        QueryFilters = GetDefaultQueryFilters(false)
            .ToDictionary(e => e.Key, e => queryFiltersEnabled.Contains(e.Key));

        try
        {
            return func();
        }
        finally
        {
            QueryFilters = currentQueryFilters;
        }
    }

    public Task ExecuteWithDisabledQueryFilters(Func<Task> action, params QueryFilter[] queryFiltersDisabled)
    {
        var currentQueryFilters = QueryFilters;
        QueryFilters = GetDefaultQueryFilters(true)
            .ToDictionary(e => e.Key, e => !queryFiltersDisabled.Contains(e.Key));

        try
        {
            return action();
        }
        finally
        {
            QueryFilters = currentQueryFilters;
        }
    }

    public Task<T> ExecuteWithDisabledQueryFilters<T>(Func<Task<T>> func,
        params QueryFilter[] queryFiltersDisabled)
    {
        var currentQueryFilters = QueryFilters;
        QueryFilters = GetDefaultQueryFilters(true)
            .ToDictionary(e => e.Key, e => !queryFiltersDisabled.Contains(e.Key));

        try
        {
            return func();
        }
        finally
        {
            QueryFilters = currentQueryFilters;
        }
    }

    public new Task SaveChanges()
    {
        ApplyEntityConcepts();

        try
        {
            return base.SaveChangesAsync();
        }
        catch
        {
            RejectChanges();
            throw;
        }
    }

    public void RejectChanges()
    {
        var entities = ChangeTracker.Entries()
            .Where(e => e.State != EntityState.Unchanged)
            .ToList();

        foreach (var entity in entities)
        {
            switch (entity.State)
            {
                case EntityState.Modified:
                case EntityState.Deleted:
                    entity.State = EntityState.Modified;
                    entity.State = EntityState.Unchanged;
                    break;
                case EntityState.Added:
                    entity.State = EntityState.Detached;
                    break;
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ApplyConfigurations(modelBuilder);
        ConfigureQueryFilters(modelBuilder);
        ConfigureDbFunctions(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private static void ApplyConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LacosDbContext).Assembly);
    }

    private static void ConfigureDbFunctions(ModelBuilder modelBuilder)
    {
        // CONCAT(REPLICATE(0, length - LEN(Number)), Number, '/', Year)
        modelBuilder.HasDbFunction(typeof(CustomDbFunctions).GetMethod(nameof(CustomDbFunctions.FormatCode))!)
            .HasTranslation(args =>
            {
                var stringType = new StringTypeMapping("nvarchar", DbType.String);
                var intType = new IntTypeMapping("int");
                // LEN(number)
                var numberLen = new SqlFunctionExpression("LEN", new[] { args[0] }, false, new[] { false }, typeof(int), intType);
                // length - LEN(number)
                var zeroCount = new SqlBinaryExpression(ExpressionType.Subtract, args[2], numberLen, typeof(int), intType);
                // 0
                var zero = new SqlConstantExpression(Expression.Constant(0), intType);
                // REPLICATE(0, 3 - LEN(number))
                var replicate = new SqlFunctionExpression("REPLICATE", new SqlExpression[] { zero, zeroCount }, false, new[] { false, false }, typeof(string), stringType);
                // '/'
                var slash = new SqlConstantExpression(Expression.Constant("/"), stringType);
                // CONCAT(REPLICATE(0, 3 - LEN(Number)), Number, '/', Year)
                var concat = new SqlFunctionExpression("CONCAT", new [] { replicate, args[0], slash, args[1] }, false, new[] { false, false, false, false }, typeof(string), stringType);

                return concat;
            });
    }

    private void ConfigureQueryFilters(ModelBuilder modelBuilder)
    {
        var entityTypes = modelBuilder.Model.GetEntityTypes();
        var configureQueryFilterMethod = typeof(LacosDbContext)
            .GetMethod(nameof(ConfigureQueryFilter), BindingFlags.Instance | BindingFlags.NonPublic);

        foreach (var entityType in entityTypes)
        {
            configureQueryFilterMethod?
                .MakeGenericMethod(entityType.ClrType)
                .Invoke(this, new object[] { modelBuilder });
        }
    }

    private void ConfigureQueryFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class
    {
        var expressions = new List<Expression<Func<TEntity, bool>>>();
        var entityType = typeof(TEntity);

        if (typeof(ISoftDelete).IsAssignableFrom(entityType))
        {
            Expression<Func<TEntity, bool>> expr = e =>
                QueryFilters[QueryFilter.SoftDelete] && !((ISoftDelete)e).IsDeleted ||
                !QueryFilters[QueryFilter.SoftDelete];

            expressions.Add(expr);
        }

        if (typeof(IOperatorEntity).IsAssignableFrom(entityType))
        {
            Expression<Func<TEntity, bool>> expr = e =>
                QueryFilters[QueryFilter.OperatorEntity] && ((IOperatorEntity)e).OperatorId == session.CurrentUser.OperatorId ||
                !QueryFilters[QueryFilter.OperatorEntity];

            expressions.Add(expr);
        }

        if (expressions.Any())
        {
            var combinedExpressions = CombineExpressions(expressions);

            modelBuilder.Entity(entityType)
                .HasQueryFilter(combinedExpressions);
        }
    }

    private static Expression<Func<TEntity, bool>> CombineExpressions<TEntity>(
        IReadOnlyCollection<Expression<Func<TEntity, bool>>> expressions)
        where TEntity : class
    {
        var combinedExpressions = expressions.First();

        foreach (var expression in expressions.Skip(1))
        {
            var parameter = Expression.Parameter(typeof(TEntity));

            var leftVisitor = new ReplaceExpressionVisitor(combinedExpressions.Parameters[0], parameter);
            var left = leftVisitor.Visit(combinedExpressions.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expression.Parameters[0], parameter);
            var right = rightVisitor.Visit(expression.Body);

            var and = Expression.AndAlso(
                left ?? throw new InvalidOperationException(),
                right ?? throw new InvalidOperationException());

            combinedExpressions = Expression.Lambda<Func<TEntity, bool>>(and, parameter);
        }

        return combinedExpressions;
    }

    private void ApplyEntityConcepts()
    {
        var entries = ChangeTracker.Entries().ToArray();

        foreach (var entry in entries)
        {
            ApplyEntityConcepts(entry);
        }
    }

    private void ApplyEntityConcepts(EntityEntry entry)
    {
        switch (entry.State)
        {
            case EntityState.Added:
                ApplyAddedConcepts(entry);
                break;
            case EntityState.Modified:
                ApplyModifiedConcepts(entry);
                break;
            case EntityState.Deleted:
                ApplyDeletedConcepts(entry);
                break;
        }
    }

    private void ApplyAddedConcepts(EntityEntry entry)
    {
        var user = session.CurrentUser;

        if (entry.Entity is ILogEntity logEntity)
        {
            var newValues = entry.CurrentValues.Properties
                .ToDictionary(p => p.Name, p => entry.CurrentValues[p.Name]?.ToString());

            var log = new EntityLog()
            {
                EntityType = entry.Entity.GetType().Name,
                EntityId = 0,
                Action = entry.State.ToString(),
                Timestamp = DateTimeOffset.UtcNow,
                UserId = user?.UserId ?? 0,
                PreviousValues = null,
                NewValues = JsonSerializer.Serialize(newValues)
            };
            Set<EntityLog>().Add(log);
        }

        if (entry.Entity is AuditedEntity auditedEntity)
        {
            if (user != null)
            {
                auditedEntity.CreatedBy = user.UserName;
                auditedEntity.CreatedById = user.UserId;
            }

            auditedEntity.CreatedOn = DateTimeOffset.UtcNow;
        }
    }

    private void ApplyModifiedConcepts(EntityEntry entry)
    {
        var user = session.CurrentUser;

        if (entry.Entity is ILogEntity logEntity)
        {
            var previousValues = entry.OriginalValues.Properties
                .ToDictionary(p => p.Name, p => entry.OriginalValues[p.Name]?.ToString());

            var newValues = entry.CurrentValues.Properties
                .ToDictionary(p => p.Name, p => entry.CurrentValues[p.Name]?.ToString());

            var log = new EntityLog()
            {
                EntityType = entry.Entity.GetType().Name,
                EntityId = (long)entry.Property("Id").CurrentValue!,
                Action = entry.State.ToString(),
                Timestamp = DateTimeOffset.UtcNow,
                UserId = user?.UserId ?? 0,
                PreviousValues = JsonSerializer.Serialize(previousValues),
                NewValues = JsonSerializer.Serialize(newValues)
            };
            Set<EntityLog>().Add(log);
        }

        if (entry.Entity is AuditedEntity auditedEntity)
        {
            if (user != null)
            {
                auditedEntity.EditedBy = user.UserName;
                auditedEntity.EditedById = user.UserId;
            }

            auditedEntity.EditedOn = DateTimeOffset.UtcNow;
        }
    }

    private void ApplyDeletedConcepts(EntityEntry entry)
    {
        var user = session.CurrentUser;

        if (entry.Entity is ISoftDelete softDeleteEntity)
        {
            entry.State = EntityState.Modified;
            softDeleteEntity.IsDeleted = true;

            if (entry.Entity is FullAuditedEntity fullAuditedEntity)
            {
                if (user != null)
                {
                    fullAuditedEntity.DeletedBy = user.UserName;
                    fullAuditedEntity.DeletedById = user.UserId;
                }

                fullAuditedEntity.DeletedOn = DateTimeOffset.UtcNow;
            }
        }
    }

    private static IDictionary<QueryFilter, bool> GetDefaultQueryFilters(bool enabled)
    {
        return new Dictionary<QueryFilter, bool>
        {
            { QueryFilter.SoftDelete, enabled },
            { QueryFilter.OperatorEntity, enabled }
        };
    }

    public IQueryable<T> ExecuteStoredProcedure<T>(string storedProcedureName, params SqlParameter[] parameters) where T : class
    {
        var query = FormattableStringFactory.Create($"EXEC {storedProcedureName} {string.Join(", ", parameters.Select((p, index) => $"{{{index}}}"))}", parameters);

        return base.Set<T>().FromSqlInterpolated(query);
    }
    public IQueryable<T> ExecuteStoredProcedure<T>(string storedProcedureName) where T : class
    {
        var query = FormattableStringFactory.Create($"EXEC {storedProcedureName}");

        return base.Set<T>().FromSqlInterpolated(query);
    }

    private class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression oldValue;
        private readonly Expression newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        public override Expression Visit(Expression node)
        {
            return node == oldValue ? newValue : base.Visit(node);
        }
    }
}

public static class CustomDbFunctions
{
    public static string FormatCode(int number, int year, [NotParameterized] int length)
    {
        throw new NotSupportedException();
    }
}