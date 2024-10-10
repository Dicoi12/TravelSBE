using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TravelSBE.Models;
using TravelSBE.Services.Interfaces;
using TravelSBE.Utils;

namespace TravelSBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        // GET: api/event/{id}
        [HttpGet("{id}")]
        public async Task<ServiceResult<EventModel>> GetEventById(int id)
        {
            return await _eventService.GetEventByIdAsync(id);
        }

        // GET: api/event?city={city}&lat={latitude}&lon={longitude}
        [HttpGet]
        public async Task<ServiceResult<List<EventModel>>> GetEventsByCityOrCoords(string? city, int? lat, int? lon)
        {
            return await _eventService.GetEventByCityOrCoords(city, lat, lon);
        }

        // POST: api/event
        [HttpPost]
        public async Task<ServiceResult<EventModel>> AddEvent([FromBody] EventModel request)
        {
            if (!ModelState.IsValid)
            {
                return new ServiceResult<EventModel>
                {
                    IsSuccessful = false,
                    ValidationMessage = "Datele furnizate nu sunt valide."
                };
            }

            return await _eventService.AddEvent(request);
        }

        // PUT: api/event/{id}
        [HttpPut("{id}")]
        public async Task<ServiceResult<EventModel>> UpdateEvent(int id, [FromBody] EventModel request)
        {
            if (!ModelState.IsValid || id != request.Id)
            {
                return new ServiceResult<EventModel>
                {
                    IsSuccessful = false,
                    ValidationMessage = "Datele furnizate nu sunt valide sau id-ul nu corespunde."
                };
            }

            return await _eventService.UpdateEvent(request);
        }

        // DELETE: api/event/{id}
        [HttpDelete("{id}")]
        public async Task<ServiceResult<bool>> DeleteEvent(int id)
        {
            return await _eventService.DeleteEvent(id);
        }
    }
}
