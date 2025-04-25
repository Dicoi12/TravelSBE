using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using TravelSBE.Data;
using TravelSBE.Entity;
using TravelSBE.Models;
using TravelSBE.Services.Interfaces;
using TravelSBE.Utils;
using System.Security.AccessControl;
using TravelsBE.Models.Filters;
using NetTopologySuite.Geometries;

namespace TravelSBE.Services
{
    public class ObjectiveService : IObjectiveService
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;

        public ObjectiveService(ApplicationDbContext context, IMapper mapper, IConfiguration configuration, HttpClient httpClient)
        {
            _context = context;
            _mapper = mapper;
            _baseUrl = configuration["BaseUrl"];
            _httpClient = httpClient;
        }


        public async Task<ServiceResult<List<ObjectiveModel>>> GetObjectivesAsync(string? search)
        {
            var result = new ServiceResult<List<ObjectiveModel>>();

            var query = _context.Objectives
                .Include(x=>x.ObjectiveType)
                .Include(x => x.Images)
                .Include(x => x.Reviews);

            var list = await query.ToListAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                list = list.Where(x => x.Name.ToLower().Contains(search.ToLower())).ToList();
            }

            var objectiveModels = _mapper.Map<List<ObjectiveModel>>(list);

            foreach (var model in objectiveModels)
            {
                var originalObjective = list.First(x => x.Id == model.Id);

                // Set image URLs
                model.Images = originalObjective.Images
                    .Select(img => $"{_baseUrl}{img.FilePath}")
                    .ToList();

                // Compute the average review rating
                if (originalObjective.Reviews.Any())
                {
                    model.MedieReview = (int)Math.Round(originalObjective.Reviews.Average(r => r.Raiting));
                }
                else
                {
                    model.MedieReview = null; // No reviews, set to null or 0 if preferred
                }
            }

            result.Result = objectiveModels;
            return result;
        }


        public async Task<ServiceResult<ObjectiveModel>> GetObjectiveByIdAsync(int id)
        {
            var result = new ServiceResult<ObjectiveModel>();

            var item = await _context.Objectives
                .Include(x=>x.ObjectiveType)
                .Include(x => x.Images)
                .Include(x=>x.Reviews)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (item == null)
            {
                result.ValidationMessage = "Objective not found.";
                return result;
            }

            var objectiveModel = _mapper.Map<ObjectiveModel>(item);
            objectiveModel.Images = item.Images
                     .Select(img => $"{_baseUrl}{img.FilePath}")
                     .ToList();

            if (item.Reviews.Any())
            {
                objectiveModel.MedieReview = item.Reviews.Average(r => r.Raiting);
            }
            else
            {
                objectiveModel.MedieReview = null;
            }


            result.Result = objectiveModel;
            return result;
        }


        public async Task<ServiceResult<List<ObjectiveModel>>> GetLocalObjectives(ObjectiveFilterModel filter)
        {
            var result = new ServiceResult<List<ObjectiveModel>>();

            var query = _context.Objectives
                .Include(o => o.Images)
                .Include(o => o.Reviews)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(o => EF.Functions.Like(o.Name.ToLower(), $"%{filter.Name.ToLower()}%"));
            }

            if (filter.TypeId.HasValue && filter.TypeId != 0)
            {
                query = query.Where(o => o.Type == filter.TypeId);
            }

            if (filter.MinRating.HasValue && filter.MinRating != 0)
            {
                query = query.Where(o => o.Reviews.Any() && o.Reviews.Average(r => r.Raiting) >= filter.MinRating);
            }

            if (filter.Latitude.HasValue && filter.Longitude.HasValue)
            {
                var userLocation = new Point(filter.Longitude.Value, filter.Latitude.Value) { SRID = 4326 };

                query = query
                    .Where(o => o.Location != null) 
                    .OrderBy(o => o.Location.Distance(userLocation)); 

                if (filter.MaxDistance.HasValue)
                {
                    query = query.Where(o => o.Location.Distance(userLocation) <= filter.MaxDistance.Value * 1000); // Convertim km în metri
                }
            }

            var objectives = await query.ToListAsync();

            var objectiveModels = objectives.Select(o => new ObjectiveModel
            {
                Id = o.Id,
                Name = o.Name,
                Description = o.Description,
                Latitude = o.Location?.Y ?? 0,
                Longitude = o.Location?.X ?? 0, 
                Distance = CalculateDistanceKm(filter.Latitude.Value, filter.Longitude.Value, o.Location.Y, o.Location.X),
                FormattedDistance = filter.Latitude.HasValue && filter.Longitude.HasValue && o.Location != null
                    ? FormatDistance(CalculateDistanceKm(filter.Latitude.Value, filter.Longitude.Value, o.Location.Y, o.Location.X))
                    : "N/A",
                City = o.City,
                Images = o.Images.Select(img => $"{_baseUrl}{img.FilePath}").ToList(),
                MedieReview = o.Reviews.Any() ? o.Reviews.Average(r => r.Raiting) : (double?)null
            }).OrderBy(o => o.Distance).ToList(); 

            result.Result = objectiveModels;
            return result;
        }

        private string FormatDistance(double distanceInMeters)
        {
            if (distanceInMeters < 1000)
            {
                return $"{Math.Round(distanceInMeters)} metri";
            }
            else
            {
                return $"{Math.Round(distanceInMeters / 1000, 1)} km";
            }
        }



        public async Task<ServiceResult<ObjectiveModel>> CreateObjectiveAsync(ObjectiveModel objective)
        {
            ServiceResult<ObjectiveModel> result = new();
            if (String.IsNullOrEmpty(objective.Name) && String.IsNullOrEmpty(objective.City) && objective.Latitude == null && objective.Longitude == null)
            {
                result.ValidationMessage = "Nu ati completat toate campurile pentru a puta adauga un obiectiv";
                result.IsSuccessful = false;
                return result;
            }
            if (string.IsNullOrEmpty(objective.Description))
            {
                objective.Description = await GenerateObjectiveDetailsAsync(objective.Name, objective.City);
            }
            objective.CreatedAt = DateTime.UtcNow;
            objective.UpdatedAt = DateTime.UtcNow;
            var mapped = _mapper.Map<Objective>(objective);
            _context.Objectives.Add(mapped);
            await _context.SaveChangesAsync();
            result.Result = _mapper.Map<ObjectiveModel>(mapped);
            return result;
        }

        public async Task<ServiceResult<ObjectiveModel>> UpdateObjectiveAsync(Objective objective)
        {
            ServiceResult<ObjectiveModel> result = new();
            if (!_context.Objectives.Any(e => e.Id == objective.Id))
            {
                return new();
            }

            objective.UpdatedAt = DateTime.UtcNow;
            _context.Entry(objective).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            result.Result = _mapper.Map<ObjectiveModel>(objective);
            return result;
        }

        public async Task<ServiceResult<bool>> DeleteObjectiveAsync(int id)
        {
            ServiceResult<bool> result = new();
            var objective = await _context.Objectives.FindAsync(id);
            if (objective == null)
            {
                result.Result = false;
            }

            _context.Objectives.Remove(objective);
            await _context.SaveChangesAsync();
            result.Result = true;
            return result;
        }
        double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371.0; // raza Pământului în km
            var dLat = (lat2 - lat1) * Math.PI / 180;
            var dLon = (lon2 - lon1) * Math.PI / 180;
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
        private async Task<string> GenerateObjectiveDetailsAsync(string name, string city)
        {
            string prompt = $"Descrie succint obiectivul turistic '{name}' din '{city}' în română.";

            var requestBody = new
            {
                model = "phi-4", // Schimbă cu modelul ales
                messages = new[]
                {
            new { role = "system", content = "Oferă răspunsuri scurte și precise despre turism." },
            new { role = "user", content = prompt }
        }
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("http://localhost:1234/v1/chat/completions", requestContent);

            if (!response.IsSuccessStatusCode)
                return "Eroare generare descriere.";

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonDocument.Parse(responseContent);

            return result.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "Descriere indisponibilă.";
        }

        public  ServiceResult<bool> InsertDefaultObjectives()
        {
            var result = new ServiceResult<bool>();
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
                bool exists = _context.Objectives.Any(o => o.Name == obj.Name);

                if (!exists)
                {
                    _context.Objectives.Add(obj);
                }
            }
            try
            {

                _context.SaveChanges();
                result.Result = true;
                return result;
            }
            catch(Exception ex)
            {
                result.Result = false;
                result.ValidationMessage = ex.Message;
                return result;
            }
        }
        public async Task UpdateMissingLocationsAsync()
        {
            // Selectăm toate obiectivele care nu au locația setată, dar au latitudine și longitudine valide
            var objectivesToUpdate = await _context.Objectives
                .Where(o => o.Location == null && o.Latitude != 0 && o.Longitude != 0)
                .ToListAsync();

            foreach (var objective in objectivesToUpdate)
            {
                // Setăm proprietatea Location folosind latitudinea și longitudinea
                objective.Location = new Point(objective.Longitude, objective.Latitude) { SRID = 4326 };
            }

            if (objectivesToUpdate.Any())
            {
                // Salvăm modificările în baza de date
                await _context.SaveChangesAsync();
            }
        }
    }
}
