using Asp.Versioning;
using HallOfFameWebApi.Entities;
using HallOfFameWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HallOfFameWebApi.Controllers
{
    [Route("api/v{version:apiVersion}/persons")]
    [ApiController]
    [ApiVersion("1.0")]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _service;
        private readonly ILogger<PersonController> _logger;

        public PersonController(IPersonService service, ILogger<PersonController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetPersons()
        {
            IEnumerable<Person> persons;

            try
            {
                persons = await _service.GetPersons();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get persons");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return Ok(persons);
        }
    }
}
