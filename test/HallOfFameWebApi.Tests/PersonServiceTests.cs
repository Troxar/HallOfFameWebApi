using HallOfFameWebApi.Entities;
using HallOfFameWebApi.Infrastructure;
using HallOfFameWebApi.Models;
using HallOfFameWebApi.Services;
using Moq;
using Moq.EntityFrameworkCore;

namespace HallOfFameWebApi.Tests
{
    public class PersonServiceTests
    {
        #region GetPersons

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
        public async Task GetPersons_ShouldReturnExpectedPersons()
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

        #endregion

        #region CreatePerson

        [Fact]
        public async Task CreatePerson_ShouldAddPersonToDatabase()
        {
            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Persons.Add(It.IsAny<Person>()));
            mockDbContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var service = new PersonService(mockDbContext.Object);
            var command = new CreatePersonCommand();
            var result = await service.CreatePerson(command);

            mockDbContext.Verify(m => m.Persons.Add(It.IsAny<Person>()), Times.Once);
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreatePerson_ShouldReturnPersonId()
        {
            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var lastId = 123;
            mockDbContext.Setup(c => c.Persons.Add(It.IsAny<Person>()))
                .Callback<Person>(p => p.Id = ++lastId);

            var service = new PersonService(mockDbContext.Object);
            var command = new CreatePersonCommand();
            var result1 = await service.CreatePerson(command);
            var result2 = await service.CreatePerson(command);

            Assert.Equal(124, result1);
            Assert.Equal(125, result2);
        }

        #endregion
    }
}
