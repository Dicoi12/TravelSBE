using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelsBE.Models;
using TravelsBE.Services.Interfaces;
using TravelSBE.Data;
using TravelSBE.Entity;
using TravelSBE.Models;
using TravelSBE.Services.Interfaces;
using TravelSBE.Utils;

namespace TravelSBE.Services
{
    public class ItineraryService : IItineraryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ItineraryService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResult<Itinerary>> AddOrUpdateItineraryAsync(ItineraryDTO itineraryDto)
        {
            var result = new ServiceResult<Itinerary>();

            var itinerary = await _context.Itineraries
                .Include(i => i.ItineraryDetails)
                .FirstOrDefaultAsync(i => i.Id == itineraryDto.Id);

            if (itinerary == null)
            {
                itinerary = new Itinerary
                {
                    Name = itineraryDto.Name,
                    Description = itineraryDto.Description,
                    IdUser = itineraryDto.IdUser,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Itineraries.Add(itinerary);
            }
            else
            {
                itinerary.Name = itineraryDto.Name;
                itinerary.Description = itineraryDto.Description;
                itinerary.IdUser = itineraryDto.IdUser;
                itinerary.UpdatedAt = DateTime.UtcNow;

                _context.ItineraryDetails.RemoveRange(itinerary.ItineraryDetails);
            }

            foreach (var detailDto in itineraryDto.ItineraryDetails)
            {
                var detail = new ItineraryDetail
                {
                    Name = detailDto.Name,
                    Descriere = detailDto.Descriere,
                    IdObjective = detailDto.IdObjective,
                    IdEvent = detailDto.IdEvent,
                    VisitOrder = detailDto.VisitOrder,
                    Itinerary = itinerary,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                itinerary.ItineraryDetails.Add(detail);
            }

            await _context.SaveChangesAsync();
            result.Result = itinerary;
            return result;
        }

        public async Task<ServiceResult<bool>> DeleteItineraryByUser(int id, int userId)
        {
            var result = new ServiceResult<bool>();
            var itinerary = _context.Itineraries.FirstOrDefault(i => i.Id == id && i.IdUser == userId);
            if (itinerary == null)
            {
                result.ValidationMessage = "Itinerary not found";
                return result;
            }
            var itineraryDetails = _context.ItineraryDetails.Where(i => i.IdItinerary == id);
            _context.Itineraries.Remove(itinerary);
            if (itineraryDetails.Any())
            {
                _context.ItineraryDetails.RemoveRange(itineraryDetails);
            }
            try
            {
                await _context.SaveChangesAsync();
                result.IsSuccessful = true;
                result.Result = true;
                return result;
            }
            catch (Exception ex)
            {
                result.ValidationMessage = ex.Message;
                return result;
            }
        }

        public async Task<ServiceResult<List<ItineraryPageDTO>>> GetAllAsync()
        {
            var result = new ServiceResult<List<ItineraryPageDTO>>();
            var itineraries = await _context.Itineraries
                .Include(i => i.ItineraryDetails)
                    .ThenInclude(i => i.Objective)
                        .ThenInclude(o => o.Images)
                .Include(i => i.ItineraryDetails)
                    .ThenInclude(i => i.Event)
                        .ThenInclude(e => e.Images)
                .Where(i => i.ItineraryDetails.Any())
                .ToListAsync();

            var itineraryPageDTOs = itineraries.Select(itinerary => new ItineraryPageDTO
            {
                Id = itinerary.Id,
                Name = itinerary.Name,
                Description = itinerary.Description,
                ItineraryDetails = itinerary.ItineraryDetails.Select(detail => new ItineraryDetailModel
                {
                    Id = detail.Id,
                    IdObjective = detail.IdObjective,
                    Objective = detail.Objective != null ? new ObjectiveModel
                    {
                        Id = detail.Objective.Id,
                        Name = detail.Objective.Name,
                        Description = detail.Objective.Description,
                        Images = ImageHelper.ConvertToImageUrls(detail.Objective.Images)
                    } : null,
                    IdEvent = detail.IdEvent,
                    Event = detail.Event != null ? new EventModel
                    {
                        Id = detail.Event.Id,
                        Name = detail.Event.Name,
                        Description = detail.Event.Description,
                        Images = ImageHelper.ConvertToImageUrls(detail.Event.Images)
                    } : null,
                    VisitOrder = detail.VisitOrder
                }).ToArray() 
            }).ToList();

            result.Result = itineraryPageDTOs;
            return result;
        }


        public Task<ServiceResult<List<ItineraryModel>>> GetByUserId(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
