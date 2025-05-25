using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TravelSBE.Data;
using TravelSBE.Entity;
using TravelSBE.Models;
using TravelSBE.Services.Interfaces;
using TravelSBE.Utils;

namespace TravelSBE.Services
{
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;
        public EventService(ApplicationDbContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
            _baseUrl = configuration["BaseUrl"];
        }
        public async Task<ServiceResult<List<EventModel>>> GetAllEventsAsync()
        {
            var result = new ServiceResult<List<EventModel>>();

            var query = _context.Events.AsNoTracking()
.Include(x => x.Images);


            var list = await query.ToListAsync();
            //if (!string.IsNullOrWhiteSpace(search))
            //{
            //    list = list.Where(x => x.Name.ToLower().Contains(search.ToLower())).ToList();
            //}
            var objectiveModels = _mapper.Map<List<EventModel>>(list);

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

        public async Task<ServiceResult<List<EventModel>>> GetEventByCityOrCoords(string? city, double? lat, double? lon)
        {
            ServiceResult<List<EventModel>> result = new();
            if (String.IsNullOrEmpty(city) && (lat == null || lon == null))
            {
                result.ValidationMessage = "Trebuie să specifici un oraș sau coordonatele.";
                return result;
            }

            List<Event> events;

            if (!String.IsNullOrEmpty(city))
            {
                events = await _context.Events.Where(x => x.City == city).ToListAsync();
            }
            else
            {
                events = await _context.Events
                                        .Where(x => x.Latitude == lat && x.Longitude == lon)
                                        .ToListAsync();
            }

            var mapped = _mapper.Map<List<EventModel>>(events);
            result.Result = mapped;
            return result;
        }


        public async Task<ServiceResult<EventModel>> GetEventByIdAsync(int id)
        {
            ServiceResult<EventModel> result = new();
            var entity = await _context.Events.Include(x => x.Images).Where(x => x.Id == id).FirstOrDefaultAsync();

            if (entity == null)
            {
                result.ValidationMessage = "Evenimentul nu a fost găsit.";
                return result;
            }

            var mapped = _mapper.Map<EventModel>(entity);
            mapped.Images = entity.Images
                   .Select(img => $"{_baseUrl}{img.FilePath}")
                   .ToList();
            result.Result = mapped;
            return result;
        }

        public async Task<ServiceResult<EventModel>> AddEvent(EventModel request)
        {
            ServiceResult<EventModel> result = new ServiceResult<EventModel>();
            if (request != null)
            {
                var mapped = _mapper.Map<Event>(request);
                await _context.AddAsync(mapped);
                await _context.SaveChangesAsync();
            }
            return result;
        }


        public async Task<ServiceResult<EventModel>> UpdateEvent(EventModel request)
        {
            ServiceResult<EventModel> result = new ServiceResult<EventModel>();

            if (request == null)
            {
                result.ValidationMessage = "Datele evenimentului sunt invalide.";
                return result;
            }

            var entity = await _context.Events.FindAsync(request.Id);

            if (entity == null)
            {
                result.ValidationMessage = "Evenimentul nu a fost găsit.";
                return result;
            }

            // Actualizează doar dacă e diferit
            if (entity.StartDate != request.StartDate)
                entity.StartDate = request.StartDate;
            if (entity.EndDate != request.EndDate)
                entity.EndDate = request.EndDate;
            entity.City = request.City;
            entity.Country = request.Country;
            entity.Latitude = request.Latitude;
            entity.Longitude = request.Longitude;
            entity.Description = request.Description;
            entity.Name = request.Name;
            entity.IdObjective = request.IdObjective;

            await _context.SaveChangesAsync();
            result.Result = request;

            return result;
        }
        public async Task<ServiceResult<bool>> DeleteEvent(int id)
        {
            ServiceResult<bool> result = new();

            var entity = await _context.Events.FindAsync(id);
            if (entity == null)
            {
                result.ValidationMessage = "Evenimentul nu a fost găsit.";
                result.Result = false;
                return result;
            }

            _context.Events.Remove(entity);
            await _context.SaveChangesAsync();

            result.Result = true;
            return result;
        }


    }
}
