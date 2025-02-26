using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TravelsBE.Models;
using TravelsBE.Services.Interfaces;
using TravelSBE.Models;
using TravelSBE.Services.Interfaces;
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
        public async Task<ServiceResult<List<ItineraryModel>>> GetAllAsync()
        {
            return await _service.GetAllAsync();
        }
        [HttpPost]
        public async Task<ServiceResult<ItineraryModel>> AddItinerary(ItineraryDTO model)
        {
            return await _service.AddItinerary(model);
        }



    }
}
