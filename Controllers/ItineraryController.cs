using Microsoft.AspNetCore.Mvc;
using TravelsBE.Models;
using TravelsBE.Services.Interfaces;
using TravelSBE.Entity;
using TravelSBE.Utils;

namespace TravelSBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItineraryController : ControllerBase
    {
        private readonly IItineraryService _service;

        public ItineraryController(IItineraryService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<ServiceResult<List<ItineraryPageDTO>>> GetAllAsync()
        {
            return await _service.GetAllAsync();
        }
        [HttpPost]
        public async Task<ServiceResult<Itinerary>> AddOrUpdateItineraryAsync(ItineraryDTO itineraryDto)
        {
            return await _service.AddOrUpdateItineraryAsync(itineraryDto);
        }
        [HttpGet("user/{userId}")]
        public async Task<ServiceResult<List<ItineraryPageDTO>>> GetByUserId(int userId)
        {
            return await _service.GetByUserId(userId);
        }
        [HttpDelete("user/{id}")]
        public async Task<ServiceResult<bool>> DeleteItineraryByUser(int id, int userId)
        {
            return await _service.DeleteItineraryByUser(id, userId);
        }
        

    }
}
