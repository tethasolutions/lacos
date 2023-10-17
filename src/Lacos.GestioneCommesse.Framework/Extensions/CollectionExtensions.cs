using AutoMapper;

namespace Lacos.GestioneCommesse.Framework.Extensions;

public static class CollectionExtensions
{
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            collection.Add(item);
        }
    }

    public static void Merge<TSource, TDestination>(this IEnumerable<TSource> source, ICollection<TDestination> destination, Func<TSource, TDestination, bool> equalsFunc,
        Action<TSource, TDestination> afterMap, IMapperBase mapper)
    {
        // remove
        var toDelete = destination
            .Where(d => source.All(s => !equalsFunc(s, d)))
            .ToArray();

        foreach (var entity in toDelete) destination.Remove(entity);

        var initialDestination = destination.ToList();

        // update/insert
        foreach (var dto in source)
        {
            var entity = initialDestination
                .SingleOrDefault(e => equalsFunc(dto, e));

            // insert
            if (entity == null)
            {
                entity = mapper.Map<TDestination>(dto);

                destination.Add(entity);
            }
            // update
            else
            {
                mapper.Map(dto, entity);
            }

            afterMap?.Invoke(dto, entity);
        }
    }
}