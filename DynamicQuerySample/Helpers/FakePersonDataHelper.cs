using DynamicQuerySample.Entities;

namespace DynamicQuerySample.Helpers;

public static class FakePersonDataHelper
{
    public static IQueryable<Person> GetFakePersonDataAsQueryable()
    {
        var persons = new List<Person>();
        for (var i = 0; i < 1000; i++)
        {
            persons.Add(new Person
            {
                Id = Faker.Identification.UkNationalInsuranceNumber(),
                Name = Faker.Name.FullName(),
                Age = Faker.RandomNumber.Next(15, 60),
                City = Faker.Address.City()
            });
        }
        
        return persons.AsQueryable();
    }
}