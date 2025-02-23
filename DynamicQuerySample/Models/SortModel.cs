namespace DynamicQuerySample.Models;

public class SortModel
{
    public string Field { get; set; } = string.Empty;
    public SortDirection Direction { get; set; } = SortDirection.Ascending; 
}

public enum SortDirection
{
    Ascending = 0,
    Descending = 1
}