using Asp.Versioning;
using HallOfFameWebApi.Entities;
using HallOfFameWebApi.Filters;
using HallOfFameWebApi.Models;
using HallOfFameWebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HallOfFameWebApi.Controllers
{
    [Route("api/v{version:apiVersion}/persons")]
    [ApiController]
    [ApiVersion("1.0")]
    [TypeFilter<HandleExceptionFilter>]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _service;

        public PersonController(IPersonService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetPersons()
        {
            IEnumerable<Person> persons = await _service.GetPersons();
            return Ok(persons);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetPerson(long id)
        {
            Person? person = await _service.GetPerson(id);
            if (person is null)
            {
                return NotFound($"Person not found, id = {id}");
            }
            return Ok(person);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePerson(CreatePersonCommand cmd)
        {
            Person person = await _service.CreatePerson(cmd);
            return Ok(person);
        }

        [HttpDelete("{id:long}")]
        [EnsurePersonExists]
        public async Task<IActionResult> DeletePerson(long id)
        {
            Person person = await _service.DeletePerson(id);
            return Ok(person);
        }

        [HttpPut("{id:long}")]
        [EnsurePersonExists]
        public async Task<IActionResult> UpdatePerson(long id, UpdatePersonCommand cmd)
        {
            Person person = await _service.UpdatePerson(id, cmd);
            return Ok(person);
        }
    }
}
