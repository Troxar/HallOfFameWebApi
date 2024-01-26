using HallOfFameWebApi.Entities;
using HallOfFameWebApi.Models;

namespace HallOfFameWebApi.Services
{
    public interface IPersonService
    {
        Task<Person?> GetPerson(long id);
        Task<IEnumerable<Person>> GetPersons();
        Task<Person> CreatePerson(CreatePersonCommand cmd);
        Task<Person> DeletePerson(long id);
        Task<bool> DoesPersonExist(long id);
    }
}
