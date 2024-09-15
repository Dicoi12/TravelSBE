using TravelSBE.Models;
using TravelSBE.Utils;

namespace TravelSBE.Services.Interfaces
{
    public interface IEventService
    {
        Task<ServiceResult<EventModel>> GetEventByIdAsync(int id);
        Task<ServiceResult<List<EventModel>>> GetEventByCityOrCoords(string? city,int? lat, int? lon);
        Task<ServiceResult<EventModel>> AddEvent(EventModel request);
        Task<ServiceResult<EventModel>> UpdateEvent(EventModel request);

    }
}
