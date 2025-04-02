using TravelsBE.Models;
using TravelSBE.Entity;
using TravelSBE.Models;
using TravelSBE.Utils;

namespace TravelsBE.Services.Interfaces;

public interface IItineraryService
{
    public Task<ServiceResult<List<ItineraryPageDTO>>> GetAllAsync();
    public Task<ServiceResult<List<ItineraryPageDTO>>> GetByUserId(int userId);
    public Task<ServiceResult<bool>> DeleteItineraryByUser(int id,int userId);
    public Task<ServiceResult<Itinerary>> AddOrUpdateItineraryAsync(ItineraryDTO itineraryDto);
}
