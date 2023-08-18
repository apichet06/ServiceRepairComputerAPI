using Microsoft.EntityFrameworkCore;
using ServiceRepairComputer.Models;
 

namespace ServiceRepairComputer.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { } 
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Computer> Computers { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<Position> Positions { get; set; }
    }
    
}
