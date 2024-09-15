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
        public EventService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ServiceResult<List<EventModel>>> GetEventByCityOrCoords(string? city, int? lat, int? lon)
        {
            ServiceResult<List<EventModel>> result = new();
            if (String.IsNullOrEmpty(city) || (lat == null && lon == null))
            {
                result.ValidationMessage = "Oras invalid";
                return result;
            }
            if (String.IsNullOrEmpty(city))
            {
                var events = await _context.Events.Where(x => x.City == city).ToListAsync();
                var mapped = _mapper.Map<List<EventModel>>(events);
                result.Result = mapped;
                return result;
            }

            return result;
        }

        public Task<ServiceResult<EventModel>> GetEventByIdAsync(int id)
        {
            throw new NotImplementedException();
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
            if (request != null)
            {
                var entity = await _context.Events.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
                entity.StartDate = request.StartDate;
                entity.EndDate = request.EndDate;
                entity.City = request.City;
                entity.Country = request.Country;
                entity.Latitude = request.Latitude;
                entity.Longitude = request.Longitude;
                entity.Description = request.Description;
                entity.Name = request.Name;
                entity.IdObjective = request.IdObjective;
                result.Result = request;
            }
            await _context.SaveChangesAsync();
            return result;
        }
    }
}
