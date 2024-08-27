using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.AccessControl;
using TravelSBE.Entity;

namespace TravelSBE.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<Objective> Objectives { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
