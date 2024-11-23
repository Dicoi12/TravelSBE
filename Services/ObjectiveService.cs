using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelSBE.Data;
using TravelSBE.Entity;
using TravelSBE.Models;
using TravelSBE.Services.Interfaces;
using TravelSBE.Utils;

namespace TravelSBE.Services
{
    public class ObjectiveService : IObjectiveService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;

        public ObjectiveService(ApplicationDbContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _baseUrl = configuration["BaseUrl"];
        }


        public async Task<ServiceResult<List<ObjectiveModel>>> GetObjectivesAsync()
        {
            var result = new ServiceResult<List<ObjectiveModel>>();

            var list = await _context.Objectives
                .Include(x => x.Images)
                .ToListAsync();

            var objectiveModels = _mapper.Map<List<ObjectiveModel>>(list);

            foreach (var model in objectiveModels)
            {
                var originalObjective = list.First(x => x.Id == model.Id);
                model.Images = originalObjective.Images
                    .Select(img => $"{_baseUrl}{img.FilePath}")
                    .ToList();
            }

            result.Result = objectiveModels;
            return result;
        }

        public async Task<ServiceResult<ObjectiveModel>> GetObjectiveByIdAsync(int id)
        {
            var result = new ServiceResult<ObjectiveModel>();

            var item = await _context.Objectives
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (item == null)
            {
                result.ValidationMessage = "Objective not found.";
                return result;
            }

            var objectiveModel = _mapper.Map<ObjectiveModel>(item);

            objectiveModel.Images = item.Images
                .Select(img => $"/wwwroot{img.FilePath}")
                .ToList();

            result.Result = objectiveModel;
            return result;
        }


        public async Task<ServiceResult<List<ObjectiveModel>>> GetLocalObjectives(double latitude, double longitude)
        {
            var result = new ServiceResult<List<ObjectiveModel>>();

            var list = await _context.Objectives
                .Include(x => x.Images)
                .ToListAsync();

            var objectiveModels = _mapper.Map<List<ObjectiveModel>>(list);

            foreach (var objective in objectiveModels)
            {
                var originalObjective = list.First(x => x.Id == objective.Id);
                objective.Images = originalObjective.Images
                    .Select(img => $"{_baseUrl}{img.FilePath}")
                    .ToList();

                var distance = CalculateDistance(latitude, longitude, (double)objective.Latitude, (double)objective.Longitude);
                objective.Distance = distance;
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
            objective.CreatedAt = DateTime.UtcNow;
            objective.UpdatedAt = DateTime.UtcNow;
            var mapped = _mapper.Map<Objective>(objective);
            _context.Objectives.Add(mapped);
            await _context.SaveChangesAsync();
            result.Result = _mapper.Map<ObjectiveModel>(mapped);
            return result;
        }

        public async Task<ServiceResult<ObjectiveModel>> UpdateObjectiveAsync(ObjectiveModel objective)
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

    }
}
