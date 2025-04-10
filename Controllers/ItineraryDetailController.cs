using Microsoft.AspNetCore.Mvc;
using TravelsBE.Services.Interfaces;
using TravelSBE.Entity;
using TravelSBE.Models;
using TravelSBE.Utils;

namespace TravelSBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItineraryDetailController : ControllerBase
    {
        private readonly IItineraryDetailService _service;

        public ItineraryDetailController(IItineraryDetailService service)
        {
            _service = service;
        }

        [HttpPost("AddItineraryDetailAsync")]
        public async Task<ServiceResult<ItineraryDetail>> AddItineraryDetailAsync([FromBody] ItineraryDetailModel itineraryDetailDto)
        {
            return await _service.AddItineraryDetailAsync(itineraryDetailDto);
        }

        [HttpPut("UpdateItineraryDetailAsync")]
        public async Task<ServiceResult<ItineraryDetail>> UpdateItineraryDetailAsync([FromBody] ItineraryDetailModel itineraryDetailDto)
        {
            return await _service.UpdateItineraryDetailAsync(itineraryDetailDto);
        }

        [HttpDelete("{id}")]
        public async Task<ServiceResult<bool>> DeleteItineraryDetailAsync(int id)
        {
            return await _service.DeleteItineraryDetailAsync(id);
        }

        [HttpGet("{id}")]
        public async Task<ServiceResult<ItineraryDetail>> GetItineraryDetailByIdAsync(int id)
        {
            return await _service.GetItineraryDetailByIdAsync(id);
        }

        [HttpGet("GetAllItineraryDetailsAsync")]
        public async Task<ServiceResult<List<ItineraryDetail>>> GetAllItineraryDetailsAsync()
        {
            return await _service.GetAllItineraryDetailsAsync();
        }
    }
}

