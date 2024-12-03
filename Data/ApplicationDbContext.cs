using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection.Metadata;
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
        public DbSet<Event> Events { get; set; }
        public DbSet<Itinerary> Itineraries { get; set; }
        public DbSet<ItineraryDetail> ItineraryDetails { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<ObjectiveImage> ObjectiveImages { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            builder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            builder.Entity<User>()
            .HasIndex(u => u.Phone)
            .IsUnique();

            builder.Entity<Objective>()
                   .HasMany(e => e.Images)
                   .WithOne(e => e.Objective)
                   .HasForeignKey(e => e.IdObjective)
                   .IsRequired();

            builder.Entity<Itinerary>()
                .HasMany(e => e.Images)
                   .WithOne(e => e.Itinerary)
                   .HasForeignKey(e => e.IdItinerary)
                   .IsRequired();
        }



    }
}
