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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
               // .ToTable("Users", schema: "security"); // Especificar el esquema de la tabla de usuarios
            .ToTable("Users");
            base.OnModelCreating(modelBuilder);
        }

    }

    public class CommonDbContext : DbContext
    {
        public CommonDbContext(DbContextOptions<CommonDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }

    }
    public class ApplicationDbContextNewCustomer : DbContext
    {
        public ApplicationDbContextNewCustomer(DbContextOptions<ApplicationDbContextNewCustomer> options)
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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
            // .ToTable("Users", schema: "security"); // Especificar el esquema de la tabla de usuarios
            .ToTable("Users");
            base.OnModelCreating(modelBuilder);
        }

    }
    
}
