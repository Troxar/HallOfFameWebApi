using HallOfFameWebApi.Entities;

namespace HallOfFameWebApi.Services
{
    public interface IPersonService
    {
        Task<IEnumerable<Person>> GetPersons();
    }
}
