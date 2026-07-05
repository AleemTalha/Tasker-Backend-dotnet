using Microsoft.EntityFrameworkCore;
using Models;

namespace Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Category> Category => Set<Category>();
        public DbSet<User> User => Set<User>();
        public DbSet<ToDoTask> Task => Set<ToDoTask>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<Category>().HasMany(c => c.tasks).WithOne(t => t.category).HasForeignKey(t => t.CategoryId);
            modelBuilder.Entity<Category>().HasIndex(c => c.Name).IsUnique();
            modelBuilder.Entity<Category>().Property(c => c.Name).HasMaxLength(40).IsRequired();

            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>().Property(u => u.Email).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<User>().Property(u => u.Password).IsRequired().HasMaxLength(220);
            modelBuilder.Entity<User>().Property(u => u.FullName).IsRequired().HasMaxLength(100);


            modelBuilder.Entity<ToDoTask>().ToTable("Tasks");
            modelBuilder.Entity<ToDoTask>().
            HasOne(t => t.user)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}