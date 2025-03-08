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
                .Include(x => x.Images)
                .Include(x => x.Reviews)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(o => o.Name.Contains(filter.Name));
            }

            if (filter.TypeId.HasValue)
            {
                query = query.Where(o => o.Type == filter.TypeId);
            }

            if (filter.MinRating.HasValue)
            {
                query = query.Where(o => o.Reviews.Average(r => r.Raiting) >= filter.MinRating);
            }

            var list = await query.ToListAsync();
            var objectiveModels = _mapper.Map<List<ObjectiveModel>>(list);

            foreach (var objective in objectiveModels)
            {
                var originalObjective = list.First(x => x.Id == objective.Id);

                objective.Images = originalObjective.Images
                    .Select(img => $"{_baseUrl}{img.FilePath}")
                    .ToList();

                if (filter.Latitude.HasValue && filter.Longitude.HasValue)
                {
                    var distance = CalculateDistance(
                        filter.Latitude.Value,
                        filter.Longitude.Value,
                        (double)objective.Latitude,
                        (double)objective.Longitude);

                    objective.Distance = distance;

                    if (filter.MaxDistance.HasValue && distance > filter.MaxDistance.Value)
                    {
                        continue; // Exclude obiectivele care depășesc distanța maximă
                    }
                }
            }

            result.Result = objectiveModels.OrderBy(o => o.Distance).ToList();
            return result;
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
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371;
            var lat = (lat2 - lat1) * (Math.PI / 180);
            var lon = (lon2 - lon1) * (Math.PI / 180);

            var a = Math.Sin(lat / 2) * Math.Sin(lat / 2) +
                    Math.Cos(lat1 * (Math.PI / 180)) * Math.Cos(lat2 * (Math.PI / 180)) *
                    Math.Sin(lon / 2) * Math.Sin(lon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c; // Distance in km 
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



    }
}
