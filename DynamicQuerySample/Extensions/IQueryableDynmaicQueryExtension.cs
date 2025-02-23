using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using DynamicQuerySample.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DynamicQuerySample.Extensions;

public static class IQueryableDynmaicQueryExtension
{
    private static readonly ConcurrentDictionary<string, PropertyInfo?> PropertyCache = new();

    public static IQueryable<T> ExecuteDynamicQuery<T>(this IQueryable<T> query, List<FilterModel> filters,
        List<SortModel> sorts)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        return query.ApplyFilters(filters, parameter).ApplySorting(sorts, parameter);
    }

    private static IQueryable<T> ApplyFilters<T>(this IQueryable<T> query, List<FilterModel> filters,
        ParameterExpression parameter)
    {
        if (filters == null || !filters.Any()) return query;

        Expression? finalExpression = null;

        foreach (var filter in filters)
        {
            var propertyAccess = GetPropertyExpression<T>(parameter, filter.Field);
            if (propertyAccess is null) continue;


            var propertyType = Nullable.GetUnderlyingType(propertyAccess.Type) ?? propertyAccess.Type;

            object? convertedValue;
            try
            {
                convertedValue = ConvertJsonElement(filter.Value, propertyType);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Cannot convert value '{filter.Value}' to type '{propertyType.Name}'", ex);
            }

            Expression constant = Expression.Constant(convertedValue, propertyAccess.Type);

            if (propertyAccess.Type != constant.Type)
            {
                try
                {
                    constant = Expression.Convert(constant, propertyAccess.Type);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Cannot convert value '{filter.Value}' to type '{propertyType.Name}'", ex);
                }
            }

            Expression? filterExpression = filter.Operator switch
            {
                FilterOperator.Equals => Expression.Equal(propertyAccess, constant),
                FilterOperator.NotEquals => Expression.NotEqual(propertyAccess, constant),
                FilterOperator.GreaterThan => Expression.GreaterThan(propertyAccess, constant),
                FilterOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(propertyAccess, constant),
                FilterOperator.LessThan => Expression.LessThan(propertyAccess, constant),
                FilterOperator.LessThanOrEqual => Expression.LessThanOrEqual(propertyAccess, constant),
                FilterOperator.Contains => Expression.Call(propertyAccess,
                    typeof(string).GetMethod("Contains", [typeof(string)])!, constant),
                FilterOperator.StartsWith => Expression.Call(propertyAccess,
                    typeof(string).GetMethod("StartsWith", [typeof(string)])!, constant),
                FilterOperator.EndsWith => Expression.Call(propertyAccess, typeof(string).GetMethod("EndsWith", [
                    typeof(string)
                ])!, constant),
                _ => throw new NotSupportedException($"Operator '{filter.Operator}' is not supported.")
            };

            finalExpression = finalExpression == null
                ? filterExpression
                : filter.Logic == FilterLogic.And
                    ? Expression.AndAlso(finalExpression, filterExpression)
                    : Expression.Or(finalExpression, filterExpression);
        }

        if (finalExpression != null)
        {
            var lambda = Expression.Lambda<Func<T, bool>>(finalExpression, parameter);
            query = query.Where(lambda);
        }

        return query;
    }

    private static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, List<SortModel> sorts,
        ParameterExpression parameter)
    {
        if (sorts == null || !sorts.Any()) return query;

        bool isFirstSort = true;

        foreach (var sort in sorts)
        {
            var propertyAccess = GetPropertyExpression<T>(parameter, sort.Field);
            if (propertyAccess is null) continue;

            var orderByExpression = Expression.Lambda(propertyAccess, parameter);

            var methodName = sort.Direction == SortDirection.Descending
                ? isFirstSort ? "OrderByDescending" : "ThenByDescending"
                : isFirstSort ? "OrderBy" : "ThenBy";

            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                [query.ElementType, propertyAccess.Type],
                query.Expression,
                Expression.Quote(orderByExpression));

            query = query.Provider.CreateQuery<T>(resultExpression);
            isFirstSort = false;
        }

        return query;
    }

    private static Expression? GetPropertyExpression<T>(ParameterExpression parameter, string propertyName)
    {
        var key = $"{typeof(T).FullName}.{propertyName}";

        var property = PropertyCache.GetOrAdd(key, _ =>
            typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance));

        return property != null ? Expression.Property(parameter, property) : null;
    }

    private static object? ConvertJsonElement(object? value, Type propertyType)
    {
        return value switch
        {
            JsonElement jsonElement => jsonElement.ToString() is { } str
                ? JsonConvert.DeserializeObject(str, propertyType)
                : value,
            JToken token => token.ToObject(propertyType) ??
                            throw new InvalidOperationException($"Cannot convert {value} to {propertyType}"),
            string stringValue => JsonConvert.DeserializeObject(stringValue, propertyType) ??
                                  throw new InvalidOperationException($"Cannot convert {value} to {propertyType}"),
            _ => Convert.ChangeType(value, propertyType)
        };
    }
}