using AutoMapper;
using TravelSBE.Data;
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
        public async Task<ServiceResult<EventModel>> GetEventByCityOrCoords(string? city, int? lat, int? lon)
        {
            ServiceResult<EventModel> result = new();
            if (String.IsNullOrEmpty(city) || (lat == null && lon == null))
            {
                result.ValidationMessage = "Oras invalid";
                return result;
            }

            return result;
        }

        public Task<ServiceResult<EventModel>> GetEventByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public Task<ServiceResult<EventModel>> AddEvent(EventModel request)
        {
            throw new NotImplementedException();
        }


        public Task<ServiceResult<EventModel>> UpdateEvent(EventModel request)
        {
            throw new NotImplementedException();
        }
    }
}
