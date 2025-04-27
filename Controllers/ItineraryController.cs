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

        [HttpGet("GetItineraryAsync")]
        public async Task<ServiceResult<List<ItineraryPageDTO>>> GetAllAsync()
        {
            return await _service.GetAllAsync();
        }

        [HttpPost("AddItineraryAsync")]
        public async Task<ServiceResult<Itinerary>> AddItineraryAsync([FromBody] ItineraryDTO itineraryDto)
        {
            return await _service.AddItineraryAsync(itineraryDto);
        }

        [HttpPut("UpdateItineraryAsync")]
        public async Task<ServiceResult<Itinerary>> UpdateItineraryAsync([FromBody] ItineraryDTO itineraryDto)
        {
            return await _service.UpdateItineraryAsync(itineraryDto);
        }

        [HttpGet("user/{userId}")]
        public async Task<ServiceResult<List<ItineraryPageDTO>>> GetByUserId(int userId)
        {
            return await _service.GetByUserId(userId);
        }
        [HttpGet("{id}")]
        public async Task<ServiceResult<ItineraryPageDTO>> GetByIdAsync(int id)
        {
            return await _service.GetByIdAsync(id);
        }

        [HttpDelete("{id}")]
        public async Task<ServiceResult<bool>> DeleteItineraryAsync(int id)
        {
            return await _service.DeleteItineraryAsync(id);
        }
    }
}

