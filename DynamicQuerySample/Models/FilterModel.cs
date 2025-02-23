namespace DynamicQuerySample.Models;

public class FilterModel
{
    public string Field { get; set; } = string.Empty;
    public FilterLogic Logic { get; set; } = FilterLogic.Or;
    public FilterOperator Operator { get; set; }
    public object? Value { get; set; }
}


public enum FilterOperator
{
    Equals,
    NotEquals,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    Contains,
    StartsWith,
    EndsWith
}

public enum FilterLogic
{
    Or = 0,
    And = 1,
}