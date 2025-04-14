using TravelSBE.Models;
using TravelSBE.Entity;
using TravelSBE.Utils;

namespace TravelsBE.Services.Interfaces
{
    public interface IItineraryDetailService
    {
        Task<ServiceResult<ItineraryDetail>> AddItineraryDetailAsync(ItineraryDetailModel itineraryDetailDto);
        Task<ServiceResult<ItineraryDetail>> UpdateItineraryDetailAsync(ItineraryDetailModel itineraryDetailDto);
        Task<ServiceResult<bool>> DeleteItineraryDetailAsync(int id);
        Task<ServiceResult<ItineraryDetail>> GetItineraryDetailByIdAsync(int id);
        Task<ServiceResult<List<ItineraryDetail>>> GetAllItineraryDetailsAsync();
    }
}

