using HallOfFameWebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace HallOfFameWebApi.Infrastructure
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<Skill> Skills { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Skill>()
                .HasKey(s => new { s.Name, s.PersonId });
            modelBuilder.Entity<Skill>()
                .HasOne(s => s.Person)
                .WithMany(p => p.Skills)
                .HasForeignKey(s => s.PersonId);
        }
    }
}
