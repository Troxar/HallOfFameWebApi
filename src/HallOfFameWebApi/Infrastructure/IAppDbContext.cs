using HallOfFameWebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace HallOfFameWebApi.Infrastructure
{
    public interface IAppDbContext
    {
        DbSet<Person> Persons { get; set; }
        DbSet<Skill> Skills { get; set; }

        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
