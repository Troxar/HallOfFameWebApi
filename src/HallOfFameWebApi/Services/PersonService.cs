using HallOfFameWebApi.Entities;
using HallOfFameWebApi.Infrastructure;
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

        public async Task<IEnumerable<Person>> GetPersons()
        {
            return await _context.Persons.ToListAsync();
        }
    }
}
