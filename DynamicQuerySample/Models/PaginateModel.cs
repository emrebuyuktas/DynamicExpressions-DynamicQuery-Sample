namespace DynamicQuerySample.Models;

public class PaginateModel<T>
{
    public PaginateModel()
    {
        Items = Array.Empty<T>();
    }

    public int Size { get; set; } = 10;
    public int Index { get; set; }
    public int Count { get; set; }
    public int Pages => (int)Math.Ceiling((double)Count / Size);
    public IList<T> Items { get; set; }
    public bool HasPrevious => Index > 0;
    public bool HasNext => Index + 1 < Pages;
}