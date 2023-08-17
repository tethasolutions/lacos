using System.Linq.Expressions;
using AutoMapper;
using Lacos.GestioneCommesse.Domain;

namespace Lacos.GestioneCommesse.Framework.Extensions;

public static class AutomapperExtensions
{
    private static readonly Type AuditedEntityType = typeof(AuditedEntity);
    private static readonly Type FullAuditedEntityType = typeof(FullAuditedEntity);
    private static readonly Type SoftDeleteType = typeof(ISoftDelete);

    public static IMappingExpression<TSource, TDest> MapMemberIf<TSource, TDest, TDestMember>(
        this IMappingExpression<TSource, TDest> expression,
        Expression<Func<TDest, TDestMember>> destination,
        Func<TSource, TDest, bool> condition)
    {
        return expression.ForMember(destination, e => e.Condition(condition));
    }

    public static IMappingExpression<TSource, TDest> MapMember<TSource, TSourceMember, TDest, TDestMember>(
        this IMappingExpression<TSource, TDest> expression,
        Expression<Func<TDest, TDestMember>> destination,
        Expression<Func<TSource, TSourceMember>> source)
    {
        return expression
            .ForMember(destination, z => z.MapFrom(source));
    }

    public static IMappingExpression<TSource, TDest> MapMember<TSource, TSourceMember, TDest, TDestMember>(
        this IMappingExpression<TSource, TDest> expression,
        Expression<Func<TDest, TDestMember>> destination,
        Func<TSource, TDest, TSourceMember> source)
    {
        return expression
            .ForMember(destination, z => z.MapFrom(source));
    }

    public static IMappingExpression<TSource, TDest> MapMemberWithValue<TSource, TDest, TDestMember>(
        this IMappingExpression<TSource, TDest> expression,
        Expression<Func<TDest, TDestMember>> destination,
        TDestMember value)
    {
        return expression
            .MapMember(destination, y => value);
    }

    public static IMappingExpression<TSource, TDest> Ignore<TSource, TDest, TDestMember>(
        this IMappingExpression<TSource, TDest> expression,
        Expression<Func<TDest, TDestMember>> destination)
    {
        return expression
            .ForMember(destination, z => z.Ignore());
    }

    public static IMappingExpression<TSource, TDest> Ignore<TSource, TDest>(
        this IMappingExpression<TSource, TDest> expression, string memberName)
    {
        return expression
            .ForMember(memberName, z => z.Ignore());
    }

    public static IMappingExpression<TSource, TDest> IgnoreCommonMembers<TSource, TDest>(
        this IMappingExpression<TSource, TDest> expression)
        where TDest : BaseEntity
    {
        var destType = typeof(TDest);

        expression = expression
            .Ignore(e => e.Id);

        if (AuditedEntityType.IsAssignableFrom(destType))
        {
            expression = expression
                .Ignore(nameof(AuditedEntity.CreatedOn))
                .Ignore(nameof(AuditedEntity.CreatedBy))
                .Ignore(nameof(AuditedEntity.CreatedById))
                .Ignore(nameof(AuditedEntity.EditedOn))
                .Ignore(nameof(AuditedEntity.EditedBy))
                .Ignore(nameof(AuditedEntity.EditedById));
        }

        if (FullAuditedEntityType.IsAssignableFrom(destType))
        {
            expression = expression
                .Ignore(nameof(FullAuditedEntity.DeletedBy))
                .Ignore(nameof(FullAuditedEntity.DeletedOn))
                .Ignore(nameof(FullAuditedEntity.DeletedById));
        }

        if (SoftDeleteType.IsAssignableFrom(destType))
        {
            expression = expression
                .Ignore(nameof(FullAuditedEntity.IsDeleted));
        }

        return expression;
    }

    public static T MapTo<T>(this object source, IMapper mapper)
    {
        return mapper.Map<T>(source);
    }

    public static T MapTo<T>(this object source, T destination, IMapper mapper)
    {
        return mapper.Map(source, destination);
    }

    public static IQueryable<T> Project<T>(this IQueryable queryable, IMapper mapper)
    {
        return mapper.ProjectTo<T>(queryable);
    }
}