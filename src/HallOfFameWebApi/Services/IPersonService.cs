﻿using HallOfFameWebApi.Entities;
using HallOfFameWebApi.Models;

namespace HallOfFameWebApi.Services
{
    public interface IPersonService
    {
        Task<IEnumerable<Person>> GetPersons();
        Task<long> CreatePerson(CreatePersonCommand cmd);
    }
}
