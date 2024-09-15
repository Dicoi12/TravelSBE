using Microsoft.AspNetCore.Mvc;
using TravelSBE.Services;
using TravelSBE.Services.Interfaces;

namespace TravelSBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : Controller
    {
        private readonly IEventService _eventService;
        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }


    }
}
