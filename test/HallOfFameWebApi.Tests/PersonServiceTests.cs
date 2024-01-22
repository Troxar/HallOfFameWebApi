using HallOfFameWebApi.Entities;
using HallOfFameWebApi.Infrastructure;
using HallOfFameWebApi.Services;
using Moq;
using Moq.EntityFrameworkCore;

namespace HallOfFameWebApi.Tests
{
    public class PersonServiceTests
    {
        [Fact]
        public async Task GetPersons_ShouldNotReturnPersonsIfTheyDoNotExist()
        {
            var persons = new List<Person>();
            var mock = new Mock<IAppDbContext>();
            mock.Setup(c => c.Persons).ReturnsDbSet(persons);

            var service = new PersonService(mock.Object);
            IEnumerable<Person> result = await service.GetPersons();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetPerson_ShouldReturnExpectedPersons()
        {
            var persons = new List<Person>
            {
                new Person { Id = 1 },
                new Person { Id = 2 },
                new Person { Id = 4 }
            };
            var mock = new Mock<IAppDbContext>();
            mock.Setup(c => c.Persons).ReturnsDbSet(persons);

            var service = new PersonService(mock.Object);
            IEnumerable<Person> result = await service.GetPersons();

            Assert.NotNull(result);
            Assert.Equal(persons.Count, result.Count());

            foreach (Person person in persons)
            {
                Assert.Contains(result, r => r.Id == person.Id);
            }
        }
    }
}
