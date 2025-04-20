using Microsoft.EntityFrameworkCore;
using TravelSBE.Data;
using TravelSBE.Services;
using Microsoft.OpenApi.Models;
using TravelSBE.Services.Interfaces;
using Microsoft.AspNetCore.Http.Features;
using TravelSBE.Mapper;
using TravelSBE.Controllers;
using TravelsBE.Services;
using TravelsBE.Services.Interfaces;
using Microsoft.Extensions.FileProviders;

namespace TravelSBE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure CORS to allow requests from localhost:3000
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    builder => builder.WithOrigins("http://localhost:5173")
                                      .AllowAnyMethod()
                                      .AllowAnyHeader()
                                      .AllowCredentials());
            });
            //builder.Services.AddSpaStaticFiles(configuration =>
            //{
            //    configuration.RootPath = "ClientApp/dist";
            //});

            builder.Services.AddControllers();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),x=>x.UseNetTopologySuite()));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(7100, listenOptions =>
                {
                    listenOptions.UseHttps();
                });
                options.ListenAnyIP(5094); // HTTP
                options.ListenAnyIP(5001, listenOptions => listenOptions.UseHttps());
            });

            builder.Services.Configure<FormOptions>(options => {
                options.MultipartBodyLengthLimit = 104857600; // 100 MB
            });

            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

            // Register your services here
            builder.Services.AddScoped<IObjectiveService, ObjectiveService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<IObjectiveImageService, ObjectiveImageService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<IExperienceService, ExperienceService>();
            builder.Services.AddScoped<IItineraryService, ItineraryService>();
            builder.Services.AddScoped<IObjectiveTypeService, ObjectiveTypeService>();
            builder.Services.AddHttpClient<ItineraryService>();



            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "TravelSBE API",
                    Description = "An ASP.NET Core Web API for managing travel objectives"
                });
            });

            var app = builder.Build();

            // Apply migrations at startup
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate(); 
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TravelSBE V1");
            });

            app.UseHttpsRedirection();

            app.UseCors("AllowFrontend");
            //app.UseSpaStaticFiles();
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
            app.UseStaticFiles();


            app.Run();
        }
    }
}