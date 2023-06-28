using Microsoft.EntityFrameworkCore;
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
}