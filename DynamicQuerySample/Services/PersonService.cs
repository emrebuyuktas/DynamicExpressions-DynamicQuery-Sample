using DynamicQuerySample.Entities;
using DynamicQuerySample.Extensions;
using DynamicQuerySample.Helpers;
using DynamicQuerySample.IO;
using DynamicQuerySample.Models;

namespace DynamicQuerySample.Services;

public class PersonService
{
    private static IQueryable<Person> queryable = FakePersonDataHelper.GetFakePersonDataAsQueryable();
    
    public PaginateModel<PersonDto> GetPersons(PagingInput input)
    {
        return queryable.Select(x => new PersonDto
        {
            Id = x.Id,
            Name = x.Name,
            Age = x.Age,
        }).ToPaginate(input.Index, input.Size);
    }

    public PersonDto GetPersonById(string id)
    {
        return queryable.Where(x => x.Id == id).Select(x => new PersonDto
        {
            Id = x.Id,
            Name = x.Name,
            Age = x.Age,
        }).FirstOrDefault();
    }

    public PaginateModel<PersonDto> SearchPersons(SearchInput input)
    {
        return queryable.ExecuteDynamicQuery(input.Filters, input.Sorts).Select(x => new PersonDto
        {
            Id = x.Id,
            Name = x.Name,
            Age = x.Age,
        }).ToPaginate(input.Index, input.Size);
    }
}