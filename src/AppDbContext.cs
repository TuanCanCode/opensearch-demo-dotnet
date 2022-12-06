using Microsoft.EntityFrameworkCore;
using OpenSearchDemo.Entities;

namespace OpenSearchDemo
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<EmployeeFts> EmployeesFts { get; set; }
    }
}
