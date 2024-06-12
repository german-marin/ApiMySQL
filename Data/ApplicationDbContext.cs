using ApiMySQL.Model;
using Microsoft.EntityFrameworkCore;

namespace ApiMySQL.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<MuscleGroup> MuscleGroups { get; set; }
        public DbSet<Training> Trainings { get; set; }
        public DbSet<TrainingLine> TrainingLines { get; set; }
        public DbSet<User> Users { get; set; }

    }
}
