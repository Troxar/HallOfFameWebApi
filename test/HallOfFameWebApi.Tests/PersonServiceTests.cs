using FluentAssertions;
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
        public class GetPersonsTests
        {
            [Fact]
            public async Task GetPersons_ShouldNotReturnPersonsIfTheyDoNotExist()
            {
                var persons = new List<Person>();
                Mock<IAppDbContext> mockDbContext = CreateMockDbContextWithPersons(persons);

                var service = new PersonService(mockDbContext.Object);
                IEnumerable<Person> result = await service.GetPersons();

                result.Should().NotBeNull()
                    .And.BeEmpty();
            }

            [Fact]
            public async Task GetPersons_ShouldReturnExpectedPersons()
            {
                List<Person> persons = CreateSamplePersons();
                Mock<IAppDbContext> mockDbContext = CreateMockDbContextWithPersons(persons);

                var service = new PersonService(mockDbContext.Object);
                IEnumerable<Person> result = await service.GetPersons();

                result.Should().NotBeNull()
                    .And.BeEquivalentTo(persons);
            }
        }

        public class GetPersonTests
        {
            [Fact]
            public async Task GetPerson_ShouldReturnNullIfPersonNotFound()
            {
                List<Person> persons = CreateSamplePersons();
                Mock<IAppDbContext> mockDbContext = CreateMockDbContextWithPersons(persons);

                var id = 5;
                var service = new PersonService(mockDbContext.Object);
                var result = await service.GetPerson(id);

                result.Should().BeNull();
            }

            [Fact]
            public async Task GetPerson_ShouldReturnPersonWithSkills()
            {
                List<Person> persons = CreateSamplePersons();
                DbContextOptions<AppDbContext> options = InitializeInMemoryDatabase(persons);

                var id = 4;
                var expected = persons.Single(r => r.Id == id);

                using (var context = new AppDbContext(options))
                {
                    var service = new PersonService(context);
                    Person? result = await service.GetPerson(id);

                    AssertPersonEquality(expected, result);
                }
            }
        }

        public class CreatePersonTests
        {
            [Fact]
            public async Task CreatePerson_ShouldAddPersonToDatabase()
            {
                List<Person> persons = CreateSamplePersons();
                DbContextOptions<AppDbContext> options = InitializeInMemoryDatabase(persons);

                Person result;
                using (var context = new AppDbContext(options))
                {
                    var service = new PersonService(context);
                    var command = SampleCreatePersonCommand;
                    result = await service.CreatePerson(command);
                }

                using (var context = new AppDbContext(options))
                {
                    bool personCreated = await context.Persons.AnyAsync(p => p.Id == result.Id);
                    personCreated.Should().BeTrue();
                }
            }

            [Fact]
            public async Task CreatePerson_ShouldReturnCreatedPerson()
            {
                List<Person> persons = CreateSamplePersons();
                DbContextOptions<AppDbContext> options = InitializeInMemoryDatabase(persons);

                var command = SampleCreatePersonCommand;

                using (var context = new AppDbContext(options))
                {
                    var service = new PersonService(context);
                    Person result = await service.CreatePerson(command);

                    AssertCommandAndPersonEquality(command, result);
                }
            }

            private CreatePersonCommand SampleCreatePersonCommand =
                new CreatePersonCommand
                {
                    Name = "Name",
                    DisplayName = "DName",
                    Skills = [
                        new CreateSkillCommand { Name = "skill#1", Level = 9 },
                        new CreateSkillCommand { Name = "skill#2", Level = 1 }
                    ]
                };
        }

        public class DeletePersonTests
        {
            [Fact]
            public async Task DeletePerson_ShouldThrowExceptionIfPersonNotFound()
            {
                List<Person> persons = CreateSamplePersons();
                Mock<IAppDbContext> mockDbContext = CreateMockDbContextWithPersons(persons);

                var id = 5;
                var service = new PersonService(mockDbContext.Object);

                var action = async () => await service.DeletePerson(id);
                await action.Should().ThrowAsync<PersonNotFoundException>();
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

                AssertPersonEquality(expected, result);
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
                    personExists.Should().BeFalse();
                }
            }
        }

        public class DoesPersonExistsTests
        {
            [Theory]
            [InlineData(2, true)]
            [InlineData(5, false)]
            public async Task DoesPersonExists_ShouldCheckIfPersonExists(long id, bool expected)
            {
                List<Person> persons = CreateSamplePersons();
                Mock<IAppDbContext> mockDbContext = CreateMockDbContextWithPersons(persons);

                var service = new PersonService(mockDbContext.Object);
                bool result = await service.DoesPersonExist(id);

                result.Should().Be(expected);
            }
        }

        public class UpdatePersonTests
        {
            [Fact]
            public async Task UpdatePerson_ShouldThrowExceptionIfPersonNotFound()
            {
                List<Person> persons = CreateSamplePersons();
                Mock<IAppDbContext> mockDbContext = CreateMockDbContextWithPersons(persons);

                var id = 5;
                var command = SampleUpdatePersonCommand;
                var service = new PersonService(mockDbContext.Object);

                var action = async () => await service.UpdatePerson(id, command);
                await action.Should().ThrowAsync<PersonNotFoundException>();
            }

            [Fact]
            public async Task UpdatePerson_ShouldReturnUpdatedPerson()
            {
                List<Person> persons = CreateSamplePersons();
                Mock<IAppDbContext> mockDbContext = CreateMockDbContextWithPersons(persons);

                var id = 2;
                var command = SampleUpdatePersonCommand;

                var service = new PersonService(mockDbContext.Object);
                Person result = await service.UpdatePerson(id, command);

                AssertCommandAndPersonEquality(command, result);
            }

            [Fact]
            public async Task UpdatePerson_ShouldUpdatePersonInDatabase()
            {
                List<Person> persons = CreateSamplePersons();
                DbContextOptions<AppDbContext> options = InitializeInMemoryDatabase(persons);

                long id = 2;
                var command = SampleUpdatePersonCommand;

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

                    AssertCommandAndPersonEquality(command, result);
                }
            }

            private UpdatePersonCommand SampleUpdatePersonCommand =
                new UpdatePersonCommand
                {
                    Name = "Name",
                    DisplayName = "DName",
                    Skills = [
                        new UpdateSkillCommand { Name = "skill#1", Level = 9 },
                        new UpdateSkillCommand { Name = "skill#2", Level = 1 }
                    ]
                };
        }

        private static void AssertPersonEquality(Person expected, Person? actual)
        {
            actual.Should().NotBeNull()
                .And.BeEquivalentTo(expected,
                    options => options.Excluding(p => p.Skills));

            actual?.Skills.Should().BeEquivalentTo(expected.Skills,
                options => options.Excluding(s => s.Person.Skills));
        }

        private static void AssertCommandAndPersonEquality<TSkillCommand>(PersonCommandBase<TSkillCommand> command, Person? person)
            where TSkillCommand : SkillCommandBase
        {
            person.Should().NotBeNull()
                .And.BeEquivalentTo(command, options => options
                    .ExcludingMissingMembers());
            person!.Id.Should().BeGreaterThan(0);
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
                Skills = [new Skill { Name = "s_name", Level = 2 }]
            },
            new Person
            {
                Id = 4,
                Name = "p_name#4",
                DisplayName = "dn#4",
                Skills = [
                    new Skill { Name = "s_name#1", Level = 4 },
                    new Skill { Name = "s_name#2", Level = 8 }
                ]
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
