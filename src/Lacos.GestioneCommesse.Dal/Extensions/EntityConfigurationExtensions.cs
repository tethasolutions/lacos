using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Extensions;

public static class EntityConfigurationExtensions
{
    public static void ConfigureDefaultDecimalPrecision(this EntityTypeBuilder builder)
    {
        var properties = builder.Metadata
            .GetProperties()
            .Where(p => (Nullable.GetUnderlyingType(p.ClrType) ?? p.ClrType) == typeof(decimal))
            .ToArray();

        foreach (var property in properties)
        {
            property.SetColumnType("decimal");
            property.SetPrecision(14);
            property.SetScale(2);
        }
    }

    public static void ConfigureDefaultDateTimeOffsetPrecision(this EntityTypeBuilder builder)
    {
        var properties = builder.Metadata
            .GetProperties()
            .Where(p => (Nullable.GetUnderlyingType(p.ClrType) ?? p.ClrType) == typeof(DateTimeOffset))
            .ToArray();

        foreach (var property in properties) property.SetPrecision(3);
    }

    public static ReferenceCollectionBuilder OneToMany<T1, T2>(this EntityTypeBuilder<T1> builder, Expression<Func<T1, IEnumerable<T2>?>> hasManyExpression,
        Expression<Func<T2, T1?>> withOneExpression, Expression<Func<T2, long>> foreignKeyExpression)
        where T1 : class
        where T2 : class
    {
        var many = hasManyExpression.GetMemberAccess();
        var one = withOneExpression.GetMemberAccess();
        var foreignKey = foreignKeyExpression.GetMemberAccess();

        return builder
            .HasMany(many.Name)
            .WithOne(one.Name)
            .HasForeignKey(foreignKey.Name)
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);
    }

    public static ReferenceCollectionBuilder OneToMany<T1, T2>(this EntityTypeBuilder<T1> builder, Expression<Func<T1, IEnumerable<T2>?>> hasManyExpression,
        Expression<Func<T2, T1?>> withOneExpression, Expression<Func<T2, long?>> foreignKeyExpression)
        where T1 : class
        where T2 : class
    {
        var many = hasManyExpression.GetMemberAccess();
        var one = withOneExpression.GetMemberAccess();
        var foreignKey = foreignKeyExpression.GetMemberAccess();

        return builder
            .HasMany(many.Name)
            .WithOne(one.Name)
            .HasForeignKey(foreignKey.Name)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }

    public static void OneToOne<T1, T2>(this EntityTypeBuilder<T1> builder, Expression<Func<T1, T2?>> oneLeftExpression, Expression<Func<T2, T1?>> oneRightExpression,
        Expression<Func<T2, long>> foreignKeyExpression)
        where T1 : class
    {
        var left = oneLeftExpression.GetMemberAccess();
        var right = oneRightExpression.GetMemberAccess();
        var foreignKey = foreignKeyExpression.GetMemberAccess();

        builder
            .HasOne(left.Name)
            .WithOne(right.Name)
            .HasForeignKey(typeof(T2), foreignKey.Name)
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);
    }

    public static void OneToOne<T1, T2>(this EntityTypeBuilder<T1> builder, Expression<Func<T1, T2?>> oneLeftExpression, Expression<Func<T2, T1?>> oneRightExpression,
        Expression<Func<T2, long?>> foreignKeyExpression)
        where T1 : class
    {
        var left = oneLeftExpression.GetMemberAccess();
        var right = oneRightExpression.GetMemberAccess();
        var foreignKey = foreignKeyExpression.GetMemberAccess();

        builder
            .HasOne(left.Name)
            .WithOne(right.Name)
            .HasForeignKey(typeof(T2), foreignKey.Name)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }

    public static void ManyToMany<T1, T2>(this EntityTypeBuilder<T1> builder, Expression<Func<T1, IEnumerable<T2>?>> leftExpression, Expression<Func<T2, IEnumerable<T1>?>> rightExpression,
        string table, string schema)
        where T1 : class
        where T2 : class
    {
        builder
            .HasMany(leftExpression)
            .WithMany(rightExpression)
            .UsingEntity(e => e.ToTable(table, schema));
    }
}