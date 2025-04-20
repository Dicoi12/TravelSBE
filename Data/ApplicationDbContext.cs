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

    public ApplicationDbContext()
    {
        CustomMigrate();
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
            builder.Entity<User>().Property(u => u.Location).HasColumnType("geography (point)");
            builder.Entity<Objective>().Property(o => o.Location).HasColumnType("geography (point)");
            builder.Entity<Event>().Property(o => o.Location).HasColumnType("geography (point)");

    }

    protected void CustomMigrate()
    {
        InsertDefaultObjectives();
    }

    protected void InsertDefaultObjectives()
    {
        using (var context = new ApplicationDbContext())
        {
            List<Objective> objectives = new List<Objective>
        {
            new Objective { Name = "Castelul Bran", Description = "Cunoscut ca și castelul lui Dracula.", Latitude = 45.5156, Longitude = 25.3674, City = "Bran", Type = 1, Website = "https://bran-castle.com" },
            new Objective { Name = "Castelul Peleș", Description = "Reședința regală din Sinaia.", Latitude = 45.3597, Longitude = 25.5426, City = "Sinaia", Type = 1, Website = "https://peles.ro" },
            new Objective { Name = "Transfăgărășan", Description = "Cea mai spectaculoasă șosea montană.", Latitude = 45.5983, Longitude = 24.6208, City = "Sibiu", Type = 2 },
            new Objective { Name = "Salina Turda", Description = "Una dintre cele mai spectaculoase saline din lume.", Latitude = 46.5877, Longitude = 23.7859, City = "Turda", Type = 3, Website = "https://salinaturda.eu" },
            new Objective { Name = "Delta Dunării", Description = "Un paradis natural unic în Europa.", Latitude = 45.1636, Longitude = 29.7691, City = "Tulcea", Type = 4 },
            new Objective { Name = "Cetatea Alba Carolina", Description = "Cetate istorică în Alba Iulia.", Latitude = 46.0655, Longitude = 23.5801, City = "Alba Iulia", Type = 1 },
            new Objective { Name = "Mocănița Maramureș", Description = "Cea mai faimoasă cale ferată îngustă.", Latitude = 47.7277, Longitude = 24.4101, City = "Vișeu de Sus", Type = 5 },
            new Objective { Name = "Sarmizegetusa Regia", Description = "Capitala dacilor.", Latitude = 45.6185, Longitude = 23.3081, City = "Hunedoara", Type = 1 },
            new Objective { Name = "Castelul Corvinilor", Description = "Unul dintre cele mai frumoase castele gotice.", Latitude = 45.7493, Longitude = 22.8881, City = "Hunedoara", Type = 1 },
            new Objective { Name = "Biserica Neagră", Description = "Cea mai mare biserică gotică din România.", Latitude = 45.6410, Longitude = 25.5884, City = "Brașov", Type = 6 },
            new Objective { Name = "Cascada Bigăr", Description = "Una dintre cele mai frumoase cascade.", Latitude = 45.0130, Longitude = 21.9490, City = "Caraș-Severin", Type = 7 },
            new Objective { Name = "Palatul Culturii Iași", Description = "Clădire emblematică din Iași.", Latitude = 47.1592, Longitude = 27.5860, City = "Iași", Type = 1 },
            new Objective { Name = "Lacul Roșu", Description = "Un lac format în urma unei alunecări de teren.", Latitude = 46.7937, Longitude = 25.8002, City = "Harghita", Type = 2 },
            new Objective { Name = "Cimitirul Vesel", Description = "Un cimitir unic cu cruci colorate și mesaje amuzante.", Latitude = 47.9716, Longitude = 23.6937, City = "Săpânța", Type = 8 },
            new Objective { Name = "Vulcanii Noroioși", Description = "Fenomen geologic unic în România.", Latitude = 45.3463, Longitude = 26.7084, City = "Buzău", Type = 2 },
            new Objective { Name = "Cheile Bicazului", Description = "Un defileu spectaculos în Carpați.", Latitude = 46.7886, Longitude = 25.8419, City = "Neamț", Type = 2 },
            new Objective { Name = "Castelul Sturdza", Description = "Castel de poveste în Miclăușeni.", Latitude = 47.1280, Longitude = 26.7728, City = "Iași", Type = 1 },
            new Objective { Name = "Babele și Sfinxul", Description = "Formațiuni stâncoase misterioase.", Latitude = 45.4061, Longitude = 25.4601, City = "Bușteni", Type = 2 },
            new Objective { Name = "Lacul Bâlea", Description = "Un lac glaciar spectaculos.", Latitude = 45.6086, Longitude = 24.6081, City = "Sibiu", Type = 2 },
            new Objective { Name = "Cetatea Rupea", Description = "Cetate medievală spectaculoasă.", Latitude = 46.0386, Longitude = 25.2211, City = "Brașov", Type = 1 },
            new Objective { Name = "Mănăstirea Voroneț", Description = "Cunoscută pentru albastrul Voroneț.", Latitude = 47.5172, Longitude = 25.8625, City = "Suceava", Type = 6 },
            new Objective { Name = "Mănăstirea Sucevița", Description = "Parte din patrimoniul UNESCO.", Latitude = 47.7331, Longitude = 25.7400, City = "Suceava", Type = 6 },
            new Objective { Name = "Băile Herculane", Description = "Stațiune balneară cu ape termale.", Latitude = 44.8802, Longitude = 22.4159, City = "Caraș-Severin", Type = 9 },
            new Objective { Name = "Cascada Cailor", Description = "Cea mai înaltă cascadă din România.", Latitude = 47.6333, Longitude = 24.8000, City = "Maramureș", Type = 7 },
            new Objective { Name = "Pădurea Hoia-Baciu", Description = "Pădure misterioasă cu legende despre OZN-uri.", Latitude = 46.7687, Longitude = 23.5162, City = "Cluj", Type = 10 },
            new Objective { Name = "Castelul Banffy", Description = "Supranumit Versailles-ul Transilvaniei.", Latitude = 46.8512, Longitude = 23.7493, City = "Cluj", Type = 1 },
            new Objective { Name = "Muzeul Astra", Description = "Cel mai mare muzeu în aer liber din România.", Latitude = 45.7485, Longitude = 24.1273, City = "Sibiu", Type = 11 },
            new Objective { Name = "Palatul Parlamentului", Description = "A doua cea mai mare clădire administrativă din lume.", Latitude = 44.4271, Longitude = 26.0877, City = "București", Type = 1 }
        };

            foreach (var obj in objectives)
            {
                bool exists = context.Objectives.Any(o => o.Name == obj.Name);

                if (!exists)
                {
                    context.Objectives.Add(obj);
                }
            }

            context.SaveChanges();

        }
    }
}





