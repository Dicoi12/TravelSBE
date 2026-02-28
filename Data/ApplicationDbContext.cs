using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TravelsBE.Entity;
using TravelSBE.Entity;

namespace TravelSBE.Data;

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
    public DbSet<ClusterNeighbor> ClusterNeighbors { get; set; }

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
               .HasForeignKey(e => e.IdObjective)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Event>()
               .HasMany(e => e.Images)
               .WithOne(e => e.Event)
               .HasForeignKey(e => e.IdEvent);

        builder.Entity<Itinerary>()
               .HasMany(e => e.ItineraryDetails)
               .WithOne(e => e.Itinerary)
              .HasForeignKey(d => d.IdItinerary)
         .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Experience>()
            .HasMany(e => e.Images)
            .WithOne(e => e.Experience)
            .HasForeignKey(e => e.IdExperienta).OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ObjectiveSchedule>()
            .Property(s => s.DayOfWeek)
            .HasConversion<string>();

        builder.Entity<User>().Property(u => u.Location).HasColumnType("geography (point)");
        builder.Entity<Objective>().Property(o => o.Location).HasColumnType("geography (point)");
        builder.Entity<Event>().Property(o => o.Location).HasColumnType("geography (point)");

        // Configurare pentru ClusterNeighbor
        builder.Entity<ClusterNeighbor>()
            .HasOne(cn => cn.SourceObjective)
            .WithMany()
            .HasForeignKey(cn => cn.SourceObjectiveId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ClusterNeighbor>()
            .HasOne(cn => cn.TargetObjective)
            .WithMany()
            .HasForeignKey(cn => cn.TargetObjectiveId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ClusterNeighbor>()
            .HasIndex(cn => new { cn.SourceObjectiveId, cn.TargetObjectiveId })
            .IsUnique();

        builder.Entity<ClusterNeighbor>()
            .HasIndex(cn => cn.ClusterId);
    }
}





