using HallOfFameWebApi.Entities;
using HallOfFameWebApi.Infrastructure;
using HallOfFameWebApi.Models;
using HallOfFameWebApi.Services.Exceptions;
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

        public async Task<Person> CreatePerson(CreatePersonCommand cmd)
        {
            Person person = cmd.ToPerson();
            _context.Persons.Add(person);
            await _context.SaveChangesAsync();

            return person;
        }

        public async Task<Person> DeletePerson(long id)
        {
            Person person = await GetExistingPerson(id);

            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();

            return person;
        }

        public async Task<bool> DoesPersonExist(long id)
        {
            return await _context.Persons
                .AnyAsync(p => p.Id == id);
        }

        public async Task<Person?> GetPerson(long id)
        {
            return await _context.Persons
                .Include(p => p.Skills)
                .SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Person>> GetPersons()
        {
            return await _context.Persons
                .Include(p => p.Skills)
                .ToListAsync();
        }

        public async Task<Person> UpdatePerson(long id, UpdatePersonCommand cmd)
        {
            Person person = await GetExistingPerson(id);

            cmd.UpdatePerson(person);
            _context.Persons.Update(person);
            await _context.SaveChangesAsync();

            return person;
        }

        private async Task<Person> GetExistingPerson(long id)
        {
            Person? person = await _context.Persons
                .Include(p => p.Skills)
                .SingleOrDefaultAsync(p => p.Id == id);

            if (person is null)
            {
                throw new PersonNotFoundException(id);
            }

            return person;
        }
    }
}
