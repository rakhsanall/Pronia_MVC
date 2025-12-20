using Microsoft.EntityFrameworkCore;
using MVC_App.Models;

namespace MVC_App.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Service> Services { get; set; }
    
    }
}
