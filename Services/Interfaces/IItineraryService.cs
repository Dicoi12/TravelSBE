using TravelsBE.Models;
using TravelSBE.Models;
using TravelSBE.Utils;

namespace TravelsBE.Services.Interfaces;

public interface IItineraryService
{
    public Task<ServiceResult<List<ItineraryModel>>> GetAllAsync();
    public Task<ServiceResult<List<ItineraryModel>>> GetByUserId(int userId);
    public Task<ServiceResult<ItineraryModel>>AddItineraryByUser(ItineraryModel model);
    public Task<ServiceResult<ItineraryModel>>EditItineraryByUser(ItineraryModel model);
    public Task<ServiceResult<bool>> DeleteItineraryByUser(int id,int userId);
    public Task<ServiceResult<ItineraryModel>> AddItinerary(ItineraryDTO model);
    public Task<string> GenerateItineraryAsync(int userId);
}
