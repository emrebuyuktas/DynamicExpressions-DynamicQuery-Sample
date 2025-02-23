using DynamicQuerySample.Models;

namespace DynamicQuerySample.Extensions;

public static class IQueryablePaginateExtensions
{
    public static PaginateModel<T> ToPaginate<T>(
        this IQueryable<T> source,
        int index,
        int size)
    {
        int count = source.Count();

        List<T> items = source
            .Skip(index * size)
            .Take(size)
            .ToList();

        var list= new PaginateModel<T>
        {
            Index = index,
            Size = size,
            Count = count,
            Items = items,
        };

        return list;
    }
}