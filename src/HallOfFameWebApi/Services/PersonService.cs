using HallOfFameWebApi.Entities;
using HallOfFameWebApi.Infrastructure;
using HallOfFameWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HallOfFameWebApi.Services
{
    public class PersonService : IPersonService
    {
        private readonly IAppDbContext _context;

        public PersonService(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<long> CreatePerson(CreatePersonCommand cmd)
        {
            Person person = cmd.ToPerson();
            _context.Persons.Add(person);
            await _context.SaveChangesAsync();

            return person.Id;
        }

        public async Task<IEnumerable<Person>> GetPersons()
        {
            return await _context.Persons
                .Include(p => p.Skills)
                .ToListAsync();
        }
    }
}
