using HallOfFameWebApi.Entities;
using HallOfFameWebApi.Infrastructure;
using HallOfFameWebApi.Models;
using HallOfFameWebApi.Services;
using HallOfFameWebApi.Services.Exceptions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
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
            Mock<IAppDbContext> mockDbContext = CreateMockDbContextWithPersons(persons);

            var service = new PersonService(mockDbContext.Object);
            IEnumerable<Person> result = await service.GetPersons();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetPersons_ShouldReturnExpectedPersons()
        {
            List<Person> persons = CreateSamplePersons();
            Mock<IAppDbContext> mockDbContext = CreateMockDbContextWithPersons(persons);

            var service = new PersonService(mockDbContext.Object);
            IEnumerable<Person> result = await service.GetPersons();

            Assert.NotNull(result);
            Assert.Equal(persons.Count, result.Count());

            foreach (Person person in persons)
            {
                Assert.Contains(result, r => r.Id == person.Id);
            }
        }

        [Fact]
        public async Task GetPersons_ShouldReturnPersonsWithSkills()
        {
            List<Person> persons = CreateSamplePersons();
            DbContextOptions<AppDbContext> options = InitializeInMemoryDatabase(persons);

            IEnumerable<Person> result;
            using (var context = new AppDbContext(options))
            {
                var service = new PersonService(context);
                result = await service.GetPersons();
            }

            Assert.NotNull(result);
            Assert.Equal(persons.Count, result.Count());

            foreach (Person expected in persons)
            {
                var actual = result.SingleOrDefault(r => r.Id == expected.Id);
                AssertPersonWithSkills(expected, actual);
            }
        }

        #endregion

        #region GetPerson

        [Fact]
        public async Task GetPerson_ShouldReturnNullIfPersonNotFound()
        {
            List<Person> persons = CreateSamplePersons();
            Mock<IAppDbContext> mockDbContext = CreateMockDbContextWithPersons(persons);

            var id = 5;
            var service = new PersonService(mockDbContext.Object);
            var result = await service.GetPerson(id);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetPerson_ShouldReturnPersonWithSkills()
        {
            List<Person> persons = CreateSamplePersons();
            DbContextOptions<AppDbContext> options = InitializeInMemoryDatabase(persons);

            var id = 4;
            var expected = persons.Single(r => r.Id == id);

            Person? result;
            using (var context = new AppDbContext(options))
            {
                var service = new PersonService(context);
                result = await service.GetPerson(id);
            }

            AssertPersonWithSkills(expected, result);
        }

        #endregion

        #region CreatePerson

        [Fact]
        public async Task CreatePerson_ShouldAddPersonToDatabase()
        {
            List<Person> persons = CreateSamplePersons();
            DbContextOptions<AppDbContext> options = InitializeInMemoryDatabase(persons);

            Person result;
            using (var context = new AppDbContext(options))
            {
                var service = new PersonService(context);
                var command = new CreatePersonCommand { Name = "Name", DisplayName = "DName" };
                result = await service.CreatePerson(command);
            }

            using (var context = new AppDbContext(options))
            {
                bool personCreated = await context.Persons.AnyAsync(p => p.Id == result.Id);
                Assert.True(personCreated);
            }
        }

        [Fact]
        public async Task CreatePerson_ShouldReturnCreatedPerson()
        {
            List<Person> persons = CreateSamplePersons();
            DbContextOptions<AppDbContext> options = InitializeInMemoryDatabase(persons);

            var command = new CreatePersonCommand
            {
                Name = "Name",
                DisplayName = "DName",
                Skills = [
                    new CreateSkillCommand { Name = "skill#1", Level = 9 },
                    new CreateSkillCommand { Name = "skill#2", Level = 1 }
                ]
            };
            Person expected = command.ToPerson();

            Person result;
            using (var context = new AppDbContext(options))
            {
                var service = new PersonService(context);
                result = await service.CreatePerson(command);
            }

            Assert.True(result.Id > 0);
            Assert.Equal(expected.Name, result.Name);
            Assert.Equal(expected.DisplayName, result.DisplayName);

            AssertPersonWithSkills(expected, result);
        }

        #endregion

        #region DeletePerson

        [Fact]
        public async Task DeletePerson_ShouldThrowExceptionIfPersonNotFound()
        {
            List<Person> persons = CreateSamplePersons();
            Mock<IAppDbContext> mockDbContext = CreateMockDbContextWithPersons(persons);

            var id = 5;
            var service = new PersonService(mockDbContext.Object);

            await Assert.ThrowsAsync<PersonNotFoundException>(async () => await service.DeletePerson(id));
        }

        [Fact]
        public async Task DeletePerson_ShouldReturnDeletedPerson()
        {
            List<Person> persons = CreateSamplePersons();
            Mock<IAppDbContext> mockDbContext = CreateMockDbContextWithPersons(persons);

            var id = 2;
            var expected = persons.Single(r => r.Id == id);

            var service = new PersonService(mockDbContext.Object);
            Person result = await service.DeletePerson(id);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);

            AssertPersonWithSkills(expected, result);
        }

        [Fact]
        public async Task DeletePerson_ShouldDeletePersonFromDatabase()
        {
            List<Person> persons = CreateSamplePersons();
            DbContextOptions<AppDbContext> options = InitializeInMemoryDatabase(persons);

            long id = 2;
            using (var context = new AppDbContext(options))
            {
                var service = new PersonService(context);
                await service.DeletePerson(id);
            }

            using (var context = new AppDbContext(options))
            {
                bool personExists = await context.Persons.AnyAsync(p => p.Id == id);
                Assert.False(personExists);
            }
        }

        #endregion

        #region DoesPersonExists

        [Theory]
        [InlineData(2, true)]
        [InlineData(5, false)]
        public async Task DoesPersonExists_ShouldCheckIfPersonExists(long id, bool expected)
        {
            List<Person> persons = CreateSamplePersons();
            Mock<IAppDbContext> mockDbContext = CreateMockDbContextWithPersons(persons);

            var service = new PersonService(mockDbContext.Object);
            bool result = await service.DoesPersonExist(id);

            Assert.Equal(expected, result);
        }

        #endregion

        #region UpdatePerson

        [Fact]
        public async Task UpdatePerson_ShouldThrowExceptionIfPersonNotFound()
        {
            List<Person> persons = CreateSamplePersons();
            Mock<IAppDbContext> mockDbContext = CreateMockDbContextWithPersons(persons);

            var id = 5;
            var cmd = new UpdatePersonCommand();
            var service = new PersonService(mockDbContext.Object);

            await Assert.ThrowsAsync<PersonNotFoundException>(async () => await service.UpdatePerson(id, cmd));
        }

        [Fact]
        public async Task UpdatePerson_ShouldReturnUpdatedPerson()
        {
            List<Person> persons = CreateSamplePersons();
            Mock<IAppDbContext> mockDbContext = CreateMockDbContextWithPersons(persons);

            var id = 2;
            var command = CreateSampleUpdatePersonCommand();
            Person expected = command.ToPerson(id);

            var service = new PersonService(mockDbContext.Object);
            Person result = await service.UpdatePerson(id, command);

            Assert.Equal(expected.Id, result.Id);
            Assert.Equal(expected.Name, result.Name);
            Assert.Equal(expected.DisplayName, result.DisplayName);

            AssertPersonWithSkills(expected, result);
        }

        [Fact]
        public async Task UpdatePerson_ShouldUpdatePersonInDatabase()
        {
            List<Person> persons = CreateSamplePersons();
            DbContextOptions<AppDbContext> options = InitializeInMemoryDatabase(persons);

            long id = 2;
            var command = CreateSampleUpdatePersonCommand();
            Person expected = command.ToPerson(id);

            using (var context = new AppDbContext(options))
            {
                var service = new PersonService(context);
                await service.UpdatePerson(id, command);
            }

            using (var context = new AppDbContext(options))
            {
                Person result = await context.Persons
                    .Include(p => p.Skills)
                    .SingleAsync(p => p.Id == id);

                Assert.Equal(expected.Id, result.Id);
                Assert.Equal(expected.Name, result.Name);
                Assert.Equal(expected.DisplayName, result.DisplayName);

                AssertPersonWithSkills(expected, result);
            }
        }

        private static UpdatePersonCommand CreateSampleUpdatePersonCommand() =>
            new UpdatePersonCommand
            {
                Name = "Name",
                DisplayName = "DName",
                Skills = [
                    new UpdateSkillCommand { Name = "skill#1", Level = 9 },
                    new UpdateSkillCommand { Name = "skill#2", Level = 1 }
                ]
            };

        #endregion

        private static void AssertPersonWithSkills(Person expected, Person? actual)
        {
            Assert.NotNull(actual);
            Assert.Equal(expected.Skills.Count, actual.Skills.Count);

            foreach (Skill skill in expected.Skills)
            {
                Assert.Contains(actual.Skills, s => s.Name == skill.Name);
            }
        }

        private static DbContextOptions<AppDbContext> InitializeInMemoryDatabase(IEnumerable<Person> persons)
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connection)
                .Options;

            using (var context = new AppDbContext(options))
            {
                context.Database.EnsureCreated();
                context.Persons.AddRange(persons);
                context.SaveChanges();
            }

            return options;
        }

        private static List<Person> CreateSamplePersons() => [
            new Person { Id = 1, Name = "p_name#1", DisplayName = "dn#1", Skills = [] },
            new Person
            {
                Id = 2,
                Name = "p_name#2",
                DisplayName = "dn#2",
                Skills = [new Skill { Name = "s_name" }]
            },
            new Person
            {
                Id = 4,
                Name = "p_name#4",
                DisplayName = "dn#4",
                Skills = [new Skill { Name = "s_name#1" }, new Skill { Name = "s_name#2" }]
            }
        ];

        private static Mock<IAppDbContext> CreateMockDbContextWithPersons(IEnumerable<Person> persons)
        {
            var mock = new Mock<IAppDbContext>();
            mock.Setup(c => c.Persons).ReturnsDbSet(persons);
            return mock;
        }
    }
}
