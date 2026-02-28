using Microsoft.EntityFrameworkCore;
using TravelSBE.Data;
using TravelSBE.Services;
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

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
            });

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.MaxDepth = 32;
                });

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    x => x.UseNetTopologySuite().CommandTimeout(180)));

            builder.Services.AddEndpointsApiExplorer();

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(7100); // ascultă pe toate interfețele
            });


            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 104857600; // 100 MB
            });

            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

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

            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();


            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await dbContext.Database.MigrateAsync();

                var mlService = scope.ServiceProvider.GetRequiredService<IMLService>();
                await mlService.TrainModelAsync();
                await mlService.UpdateClusterNeighborsAsync();

                var objectiveService = scope.ServiceProvider.GetRequiredService<IObjectiveService>();
                await objectiveService.UpdateMissingLocationsAsync();

                // Adaugă obiective implicite dacă nu există
                var defaultObjectivesResult = objectiveService.InsertDefaultObjectives();
                if (defaultObjectivesResult.Result)
                {
                    // Reantrenează modelul după adăugarea obiectivelor implicite
                    await mlService.TrainModelAsync();
                    await mlService.UpdateClusterNeighborsAsync();
                }

                var users = await dbContext.Users.ToListAsync();
                var objectives = await dbContext.Objectives.Include(x => x.Reviews).Where(x => x.Reviews.Count == 0).ToListAsync();
                var random = new Random();
                foreach (var user in users)
                {
                    foreach (var objective in objectives)
                    {
                        var existingReview = await dbContext.Reviews
                            .FirstOrDefaultAsync(r => r.IdUser == user.Id && r.IdObjective == objective.Id);

                        if (existingReview == null)
                        {
                            var review = new Review
                            {
                                IdUser = user.Id,
                                IdObjective = objective.Id,
                                Raiting = random.Next(2, 6),
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


            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowFrontend");

            app.UseStaticFiles();

            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "images")),
                RequestPath = "",
                EnableDirectoryBrowsing = true
            });

            app.UseAuthorization();

            app.MapControllers();
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}

            await app.RunAsync();
        }
    }
}
