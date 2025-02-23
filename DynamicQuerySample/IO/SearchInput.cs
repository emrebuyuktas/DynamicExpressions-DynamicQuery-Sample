using DynamicQuerySample.Models;

namespace DynamicQuerySample.IO;

public class SearchInput : PagingInput
{
    public List<FilterModel> Filters { get; set; }
    public List<SortModel> Sorts { get; set; }
}