using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TravelsBE.Entity;
using TravelsBE.Models;
using TravelsBE.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using TravelSBE.Utils;
using TravelSBE.Data;

namespace TravelsBE.Services
{
    public class ExperienceService : IExperienceService
    {
        private readonly ApplicationDbContext _context;
        private readonly AutoMapper.IMapper _mapper; 
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;

        public ExperienceService(ApplicationDbContext context, AutoMapper.IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
            _baseUrl = configuration["BaseUrl"];
        }

        public async Task<ServiceResult<List<ExperienceModel>>> GetAllExperiencesAsync()
        {
            var result = new ServiceResult<List<ExperienceModel>>();
            var query = _context.Experiences.Include(x => x.Images);

            var list = await query.ToListAsync();
            var experienceModels = _mapper.Map<List<ExperienceModel>>(list);

            foreach (var model in experienceModels)
            {
                var originalExperience = list.First(x => x.Id == model.Id);
                model.Images = originalExperience.Images
                    .Select(img => $"{_baseUrl}{img.FilePath}")
                    .ToList();
            }

            result.Result = experienceModels;
            return result;
        }

        public async Task<ServiceResult<List<ExperienceModel>>> GetExperienceByCityOrCoords(string? city, double? lat, double? lon)
        {
            var result = new ServiceResult<List<ExperienceModel>>();

            if (string.IsNullOrEmpty(city) && (lat == null || lon == null))
            {
                result.ValidationMessage = "Trebuie să specifici un oraș sau coordonatele.";
                return result;
            }

            List<Experience> experiences;

            if (!string.IsNullOrEmpty(city))
            {
                experiences = await _context.Experiences.Where(x => x.City == city)
                                                       .ToListAsync();
            }
            else
            {
                experiences = await _context.Experiences
                    .Where(x => x.Latitude == lat && x.Longitude == lon)
                    .ToListAsync();
            }

            var mapped = _mapper.Map<List<ExperienceModel>>(experiences);
            result.Result = mapped;
            return result;
        }

        public async Task<ServiceResult<ExperienceModel>> GetExperienceByIdAsync(int id)
        {
            var result = new ServiceResult<ExperienceModel>();
            var entity = await _context.Experiences.Include(x => x.Images).FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                result.ValidationMessage = "Experiența nu a fost găsită.";
                return result;
            }

            var mapped = _mapper.Map<ExperienceModel>(entity);
            mapped.Images = entity.Images
                .Select(img => $"{_baseUrl}{img.FilePath}")
                .ToList();

            result.Result = mapped;
            return result;
        }

        public async Task<ServiceResult<ExperienceModel>> AddExperience(ExperienceModel request)
        {
            var result = new ServiceResult<ExperienceModel>();

            if (request != null)
            {
                var mapped = _mapper.Map<Experience>(request);
                await _context.AddAsync(mapped);
                await _context.SaveChangesAsync();
                result.Result = _mapper.Map<ExperienceModel>(mapped);
            }

            return result;
        }

        public async Task<ServiceResult<ExperienceModel>> UpdateExperience(ExperienceModel request)
        {
            var result = new ServiceResult<ExperienceModel>();

            if (request == null)
            {
                result.ValidationMessage = "Datele experienței sunt invalide.";
                return result;
            }

            var entity = await _context.Experiences.FindAsync(request.Id);

            if (entity == null)
            {
                result.ValidationMessage = "Experiența nu a fost găsită.";
                return result;
            }

            _mapper.Map(request, entity);
            await _context.SaveChangesAsync();

            result.Result = request;
            return result;
        }

        public async Task<ServiceResult<bool>> DeleteExperience(int id)
        {
            var result = new ServiceResult<bool>();

            var entity = await _context.Experiences.FindAsync(id);
            if (entity == null)
            {
                result.ValidationMessage = "Experiența nu a fost găsită.";
                result.Result = false;
                return result;
            }

            _context.Experiences.Remove(entity);
            await _context.SaveChangesAsync();

            result.Result = true;
            return result;
        }
    }
}