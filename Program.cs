using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;
using TravelsBE.Services;
using TravelsBE.Services.Interfaces;
using TravelSBE.Data;
using TravelSBE.Mapper;
using TravelSBE.Services;
using TravelSBE.Services.Interfaces;

namespace TravelSBE
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // CORS — configurable from appsettings
            var allowedOrigins = builder.Configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>()
                ?? new[] { "http://localhost:5173" };

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });


            //builder.Services.AddCors(options =>
            //{
            //    options.AddPolicy("AllowFrontend", policy =>
            //        policy.WithOrigins(allowedOrigins)
            //              .AllowAnyMethod()
            //              .AllowAnyHeader());
            //});

            // JWT Authentication
            var jwtKey = builder.Configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key is not configured. Set Jwt:Key in appsettings or environment variables.");

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            builder.Services.AddAuthorization();

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.MaxDepth = 32;
                });

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    x => x.UseNetTopologySuite().CommandTimeout(180)));

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(7100);
            });

            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 104857600; // 100 MB
            });

            // Rate limiting: 100 requests/minute per user/IP
            builder.Services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.User?.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 100,
                            Window = TimeSpan.FromMinutes(1)
                        }));
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
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
            //builder.Services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TravelSBE API", Version = "v1" });
            //    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            //    {
            //        Name = "Authorization",
            //        Type = SecuritySchemeType.Http,
            //        Scheme = "Bearer",
            //        BearerFormat = "JWT",
            //        In = ParameterLocation.Header,
            //        Description = "Enter your JWT token. Example: eyJhbGci..."
            //    });
            //    c.AddSecurityRequirement(new OpenApiSecurityRequirement
            //    {
            //        {
            //            new OpenApiSecurityScheme
            //            {
            //                Reference = new OpenApiReference
            //                {
            //                    Type = ReferenceType.SecurityScheme,
            //                    Id = "Bearer"
            //                }
            //            },
            //            Array.Empty<string>()
            //        }
            //    });
            //});

            var app = builder.Build();

            // Global error handling
            app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });
                });
            });

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await dbContext.Database.MigrateAsync();

                var mlService = scope.ServiceProvider.GetRequiredService<IMLService>();
                await mlService.TrainModelAsync();
                await mlService.UpdateClusterNeighborsAsync();

                var objectiveService = scope.ServiceProvider.GetRequiredService<IObjectiveService>();
                await objectiveService.UpdateMissingLocationsAsync();

                var defaultObjectivesResult = objectiveService.InsertDefaultObjectives();
                if (defaultObjectivesResult.Result)
                {
                    await mlService.TrainModelAsync();
                    await mlService.UpdateClusterNeighborsAsync();
                }
            }

            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseRateLimiter();

            // Serve static files without directory browsing
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "images")),
                RequestPath = ""
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync();
        }
    }
}
