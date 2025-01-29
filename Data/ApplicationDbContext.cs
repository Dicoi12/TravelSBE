﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Security.AccessControl;
using TravelsBE.Entity;
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
        public DbSet<ObjectiveType> ObjectiveTypes { get; set; }
        public DbSet<ObjectiveSchedule> ObjectiveSchedules { get; set; }
        public DbSet<Experience> Experiences { get; set; }  
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
                   .HasForeignKey(e => e.IdObjective);

            builder.Entity<Objective>()
                   .HasMany(e => e.Reviews)
                   .WithOne(e => e.Objective)
                   .HasForeignKey(e => e.Id);

            builder.Entity<Event>()
                   .HasMany(e => e.Images)
                   .WithOne(e => e.Event)
                   .HasForeignKey(e => e.IdEvent);

            builder.Entity<Itinerary>()
                   .HasMany(e => e.ItineraryDetails)
                   .WithOne(e => e.Itinerary)
                   .HasForeignKey(e => e.Id);


            builder.Entity<ObjectiveSchedule>()
                .Property(s => s.DayOfWeek)
                .HasConversion<string>();
        }



    }
}
