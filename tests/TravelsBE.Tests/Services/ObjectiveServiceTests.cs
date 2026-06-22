using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using NetTopologySuite.Geometries;
using TravelsBE.Entity;
using TravelsBE.Models.Filters;
using TravelSBE.Data;
using TravelSBE.Entity;
using TravelSBE.Mapper;
using TravelSBE.Models;
using TravelSBE.Services;
using TravelSBE.Utils;

namespace TravelsBE.Tests.Services;

public class ObjectiveServiceTests
{
    [Fact]
    public async Task GetObjectivesAsync_ReturnsPagedObjectives_WithAbsoluteImageUrls()
    {
        await using var context = CreateContext();
        await SeedObjectives(context);
        var service = CreateService(context);

        var result = await service.GetObjectivesAsync(search: null, page: 1, pageSize: 2);

        Assert.Equal(3, result.Result.TotalCount);
        Assert.Equal(1, result.Result.Page);
        Assert.Equal(2, result.Result.PageSize);
        Assert.Equal(2, result.Result.Items.Count);
        Assert.Contains("http://localhost/uploads/bran.jpg", result.Result.Items[0].Images);
    }

    [Fact]
    public async Task GetObjectivesAsync_FiltersByNameIgnoringCase()
    {
        await using var context = CreateContext();
        await SeedObjectives(context);
        var service = CreateService(context);

        var result = await service.GetObjectivesAsync(search: "brasov", page: 1, pageSize: 10);

        var objective = Assert.Single(result.Result.Items);
        Assert.Equal("Brasov Adventure Park", objective.Name);
    }

    [Fact]
    public async Task GetObjectiveByIdAsync_ReturnsAverageRatingAndImageUrls()
    {
        await using var context = CreateContext();
        var objectives = await SeedObjectives(context);
        var service = CreateService(context);

        var result = await service.GetObjectiveByIdAsync(objectives.BranCastle.Id);

        Assert.Null(result.ValidationMessage);
        Assert.Equal("Castelul Bran", result.Result.Name);
        Assert.Equal(4.5, result.Result.MedieReview);
        Assert.Contains("http://localhost/uploads/bran.jpg", result.Result.Images);
    }

    [Fact]
    public async Task GetObjectiveByIdAsync_ReturnsValidationMessage_WhenObjectiveDoesNotExist()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.GetObjectiveByIdAsync(999);

        Assert.Null(result.Result);
        Assert.Equal("Objective not found.", result.ValidationMessage);
    }

    [Fact]
    public async Task GetLocalObjectives_FiltersByTypeAndMinimumRating()
    {
        await using var context = CreateContext();
        await SeedObjectives(context);
        var service = CreateService(context);

        var result = await service.GetLocalObjectives(new ObjectiveFilterModel
        {
            TypeId = 1,
            MinRating = 4.0
        });

        var objective = Assert.Single(result.Result);
        Assert.Equal("Castelul Bran", objective.Name);
        Assert.Equal(4.5, objective.MedieReview);
    }

    [Fact]
    public async Task GetObjectivesForModel_MapsSimplifiedObjectiveData()
    {
        await using var context = CreateContext();
        await SeedObjectives(context);
        var service = CreateService(context);

        var result = await service.GetObjectivesForModel("bran");

        var objective = Assert.Single(result);
        Assert.Equal("Castelul Bran", objective.Denumire);
        Assert.Equal("Castel turistic", objective.Descriere);
        Assert.Equal("Castel", objective.Tip);
        Assert.Equal("Bran", objective.Oras);
    }

    [Fact]
    public async Task CreateObjectiveAsync_SavesObjectiveAndUpdatesMlData()
    {
        await using var context = CreateContext();
        var mlService = new TestMlService();
        var service = CreateService(context, mlService);

        var result = await service.CreateObjectiveAsync(new ObjectiveModel
        {
            Name = "Muzeu Test",
            Description = "Descriere deja completata",
            City = "Cluj",
            Latitude = 46.7712,
            Longitude = 23.6236,
            Type = 3
        });

        var savedObjective = await context.Objectives.SingleAsync();

        Assert.Equal("Muzeu Test", result.Result.Name);
        Assert.Equal("Muzeu Test", savedObjective.Name);
        Assert.NotNull(savedObjective.Location);
        Assert.Equal(23.6236, savedObjective.Location.X, precision: 4);
        Assert.Equal(46.7712, savedObjective.Location.Y, precision: 4);
        Assert.Equal(1, mlService.TrainModelCallCount);
        Assert.Equal(1, mlService.UpdateClusterNeighborsCallCount);
    }

    private static ObjectiveService CreateService(ApplicationDbContext context, TestMlService? mlService = null)
    {
        return new ObjectiveService(
            context,
            CreateMapper(),
            CreateConfiguration(),
            new HttpClient(),
            NullLogger<ObjectiveService>.Instance,
            mlService ?? new TestMlService());
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    private static IConfiguration CreateConfiguration()
    {
        var values = new Dictionary<string, string?>
        {
            ["BaseUrl"] = "http://localhost"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }

    private static IMapper CreateMapper()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        return configuration.CreateMapper();
    }

    private static async Task<(Objective BranCastle, Objective BrasovPark, Objective Museum)> SeedObjectives(ApplicationDbContext context)
    {
        var castleType = new ObjectiveType { Name = "Castel" };
        var parkType = new ObjectiveType { Name = "Parc" };

        var branCastle = new Objective
        {
            Name = "Castelul Bran",
            Description = "Castel turistic",
            City = "Bran",
            Type = 1,
            ObjectiveType = castleType,
            Latitude = 45.5156,
            Longitude = 25.3674,
            Location = new Point(25.3674, 45.5156) { SRID = 4326 },
            Images = new List<ObjectiveImage>
            {
                new() { FilePath = "/uploads/bran.jpg", ImageMimeType = "image/jpeg" }
            },
            Reviews = new List<Review>
            {
                new() { Raiting = 4 },
                new() { Raiting = 5 }
            }
        };

        var brasovPark = new Objective
        {
            Name = "Brasov Adventure Park",
            Description = "Parc de aventura",
            City = "Brasov",
            Type = 2,
            ObjectiveType = parkType,
            Latitude = 45.6200,
            Longitude = 25.6400,
            Location = new Point(25.6400, 45.6200) { SRID = 4326 },
            Reviews = new List<Review>
            {
                new() { Raiting = 3 }
            }
        };

        var museum = new Objective
        {
            Name = "Muzeul Astra",
            Description = "Muzeu in aer liber",
            City = "Sibiu",
            Type = 3,
            Latitude = 45.7485,
            Longitude = 24.1273,
            Location = new Point(24.1273, 45.7485) { SRID = 4326 }
        };

        context.Objectives.AddRange(branCastle, brasovPark, museum);
        await context.SaveChangesAsync();

        return (branCastle, brasovPark, museum);
    }

    private sealed class TestMlService : IMLService
    {
        public int TrainModelCallCount { get; private set; }

        public int UpdateClusterNeighborsCallCount { get; private set; }

        public Task TrainModelAsync()
        {
            TrainModelCallCount++;
            return Task.CompletedTask;
        }

        public Task UpdateClusterNeighborsAsync()
        {
            UpdateClusterNeighborsCallCount++;
            return Task.CompletedTask;
        }

        public Task<ServiceResult<List<RecommendedObjectiveDto>>> GetRecommendedObjectivesAsync(int objectiveId, int count = 4)
        {
            return Task.FromResult(new ServiceResult<List<RecommendedObjectiveDto>>(new List<RecommendedObjectiveDto>()));
        }

        public Task<ServiceResult<List<ObjectiveVisualizationDto>>> GetObjectivesForVisualizationAsync()
        {
            return Task.FromResult(new ServiceResult<List<ObjectiveVisualizationDto>>(new List<ObjectiveVisualizationDto>()));
        }

        public Task<ServiceResult<List<RecommendedObjectiveDto>>> GetNearestNeighborsAsync(int objectiveId, int count = 4)
        {
            return Task.FromResult(new ServiceResult<List<RecommendedObjectiveDto>>(new List<RecommendedObjectiveDto>()));
        }

        public Task<ServiceResult<ClusterAnalysisDto>> GetClusterAnalysisAsync()
        {
            return Task.FromResult(new ServiceResult<ClusterAnalysisDto>(new ClusterAnalysisDto()));
        }
    }
}
