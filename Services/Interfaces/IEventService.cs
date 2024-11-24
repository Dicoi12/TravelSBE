using TravelSBE.Models;
using TravelSBE.Utils;

namespace TravelSBE.Services.Interfaces
{
    public interface IEventService
    {
        Task<ServiceResult<List<EventModel>>> GetAllEventsAsync();
        Task<ServiceResult<EventModel>> GetEventByIdAsync(int id);
        Task<ServiceResult<List<EventModel>>> GetEventByCityOrCoords(string? city, double? lat, double? lon);
        Task<ServiceResult<EventModel>> AddEvent(EventModel request);
        Task<ServiceResult<EventModel>> UpdateEvent(EventModel request);
        Task<ServiceResult<bool>> DeleteEvent(int id);

    }
}
