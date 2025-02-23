using DynamicQuerySample.IO;
using DynamicQuerySample.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DynamicQuerySample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Person : ControllerBase
    {
        private readonly PersonService _personService;

        public Person(PersonService personService)
        {
            _personService = personService;
        }

        [HttpGet("GetById/{id}")]
        public IActionResult Get(string id)
        {
            return Ok(_personService.GetPersonById(id));
        }

        [HttpPost("GetAll")]
        public IActionResult GetAll([FromBody]PagingInput input)
        {
            return Ok(_personService.GetPersons(input));
        }

        [HttpPost("search")]
        public IActionResult Search([FromBody]SearchInput input)
        {
            return Ok(_personService.SearchPersons(input));
        }
    }
}
