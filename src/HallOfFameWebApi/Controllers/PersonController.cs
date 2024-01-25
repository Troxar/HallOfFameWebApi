﻿using Asp.Versioning;
using HallOfFameWebApi.Entities;
using HallOfFameWebApi.Filters;
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
    }
}