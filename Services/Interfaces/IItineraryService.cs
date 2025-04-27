using TravelsBE.Models;
using TravelSBE.Entity;
using TravelSBE.Models;
using TravelSBE.Utils;

namespace TravelsBE.Services.Interfaces;

public interface IItineraryService
{
    Task<ServiceResult<Itinerary>> AddItineraryAsync(ItineraryDTO itineraryDto);
    Task<ServiceResult<Itinerary>> UpdateItineraryAsync(ItineraryDTO itineraryDto);
    Task<ServiceResult<bool>> DeleteItineraryAsync(int id);
    Task<ServiceResult<List<ItineraryPageDTO>>> GetAllAsync();
    Task<ServiceResult<List<ItineraryPageDTO>>> GetByUserId(int userId);
    Task<ServiceResult<ItineraryPageDTO>> GetByIdAsync(int id);
}

