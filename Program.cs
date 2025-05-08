using Microsoft.EntityFrameworkCore;
using TravelSBE.Data;
using TravelSBE.Services;
using Microsoft.OpenApi.Models;
using TravelSBE.Services.Interfaces;
using Microsoft.AspNetCore.Http.Features;
using TravelSBE.Mapper;
using TravelsBE.Services;
using TravelsBE.Services.Interfaces;
using Microsoft.Extensions.FileProviders;
using TravelSBE.Entity;

namespace TravelSBE
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure CORS to allow requests from localhost:5173
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                    policy.WithOrigins("http://localhost:5173")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials());
            });

            // Add controllers
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.MaxDepth = 32;
                });

            // Configure Entity Framework with NetTopologySuite
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    x => x.UseNetTopologySuite()));

            // Add Swagger for API documentation
            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo
            //    {
            //        Version = "v1",
            //        Title = "TravelSBE API",
            //        Description = "An ASP.NET Core Web API for managing travel objectives"
            //    });
            //});

            // Configure Kestrel server
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(7100, listenOptions => listenOptions.UseHttps());
                options.ListenAnyIP(5094); // HTTP
                options.ListenAnyIP(5001, listenOptions => listenOptions.UseHttps());
            });

            // Configure form options for large file uploads
            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 104857600; // 100 MB
            });

            // Add AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

            // Register application services
            builder.Services.AddScoped<IObjectiveService, ObjectiveService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<IObjectiveImageService, ObjectiveImageService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<IExperienceService, ExperienceService>();
            builder.Services.AddScoped<IItineraryService, ItineraryService>();
            builder.Services.AddScoped<IObjectiveTypeService, ObjectiveTypeService>();
            builder.Services.AddScoped<IItineraryDetailService, ItineraryDetailService>();
            builder.Services.AddScoped<IMLService, MLService>();
            builder.Services.AddHttpClient<ItineraryService>();

            var app = builder.Build();

            // Apply migrations and update missing locations at startup
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await dbContext.Database.MigrateAsync();

                // Antrenăm modelul de ML la pornire
                var mlService = scope.ServiceProvider.GetRequiredService<IMLService>();
                await mlService.TrainModelAsync();

                var objectiveService = scope.ServiceProvider.GetRequiredService<IObjectiveService>();
                await objectiveService.UpdateMissingLocationsAsync();

                // Adăugăm recenzii aleatorii pentru toți utilizatorii și obiectivele
                var users = await dbContext.Users.ToListAsync();
                var objectives = await dbContext.Objectives.ToListAsync();
                var random = new Random();

                foreach (var user in users)
                {
                    foreach (var objective in objectives)
                    {
                        // Verificăm dacă există deja o recenzie pentru această pereche user-objective
                        var existingReview = await dbContext.Reviews
                            .FirstOrDefaultAsync(r => r.IdUser == user.Id && r.IdObjective == objective.Id);

                        if (existingReview == null)
                        {
                            var review = new Review
                            {
                                IdUser = user.Id,
                                IdObjective = objective.Id,
                                Raiting = random.Next(2, 6), // Generează un număr între 2 și 5
                                DatePosted = DateTime.UtcNow,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };

                            dbContext.Reviews.Add(review);
                        }
                    }
                }

                await dbContext.SaveChangesAsync();
            }

            // Enable developer exception page in development
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable Swagger
            //app.UseSwagger();
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TravelSBE V1");
            //});

            // Enable HTTPS redirection
            app.UseHttpsRedirection();

            // Enable CORS
            app.UseCors("AllowFrontend");

            // Serve static files
            app.UseStaticFiles();

            // Serve uploaded images from a specific directory
            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "images")),
                RequestPath = "",
                EnableDirectoryBrowsing = false
            });

            // Enable authorization
            app.UseAuthorization();

            // Map controllers
            app.MapControllers();

            // Run the application
            await app.RunAsync();
        }
    }
}
