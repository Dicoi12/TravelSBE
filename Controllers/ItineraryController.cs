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



    }
}
