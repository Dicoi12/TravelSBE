using Microsoft.EntityFrameworkCore;
using TravelSBE.Data;
using TravelSBE.Services;
using TravelSBE.Mapper;
using Microsoft.OpenApi.Models;

namespace TravelSBE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Adaugă serviciile la container.
            builder.Services.AddControllers();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            // Add Swagger services
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(7100, listenOptions =>
                {
                    listenOptions.UseHttps();
                });
                options.ListenAnyIP(5094);
            });
             builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAllOrigins",
            builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
    });
            // Adaugă AutoMapper și specifică profilul de mapare
            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

            // Register your services here
            builder.Services.AddScoped<IObjectiveService, ObjectiveService>();

            // Configurarea aplicației
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
            // Configurează pipeline-ul HTTP request.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });


            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseCors("AllowAllOrigins");
            app.MapControllers();
            app.Run();
        }
    }
}