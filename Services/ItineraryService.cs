using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TravelsBE.Models;
using TravelsBE.Services.Interfaces;
using TravelSBE.Data;
using TravelSBE.Entity;
using TravelSBE.Models;
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

        public async Task<ServiceResult<Itinerary>> AddItineraryAsync(ItineraryDTO itineraryDto)
        {
            var result = new ServiceResult<Itinerary>();

            var itinerary = new Itinerary
            {
                Name = itineraryDto.Name,
                Description = itineraryDto.Description,
                IdUser = itineraryDto.IdUser,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Itineraries.Add(itinerary);
            await _context.SaveChangesAsync();

            var savedItineraryId = itinerary.Id; // Ensure the saved itinerary ID is captured  

                List<ItineraryDetail> list = new();
            foreach (var detailDto in itineraryDto.ItineraryDetails)
            {
                var detail = new ItineraryDetail
                {
                    Name = detailDto.Name,
                    Descriere = detailDto.Descriere,
                    IdObjective = detailDto.IdObjective,
                    IdEvent = detailDto.IdEvent,
                    VisitOrder = detailDto.VisitOrder,
                    IdItinerary = savedItineraryId, // Use the saved itinerary ID  
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                list.Add(detail);
            }
            _context.AddRange(list);
            try
            {
                await _context.SaveChangesAsync();
                result.Result = itinerary;
                return result;
            }
            catch (Exception ex)
            {
                result.ValidationMessage = ex.Message;
                result.IsSuccessful = false;
                return result;
            }
        }

        public async Task<ServiceResult<Itinerary>> UpdateItineraryAsync(ItineraryDTO itineraryDto)
        {
            var result = new ServiceResult<Itinerary>();

            var itinerary = await _context.Itineraries
                .FirstOrDefaultAsync(i => i.Id == itineraryDto.Id);

            if (itinerary == null)
            {
                result.ValidationMessage = "Itinerary not found";
                result.IsSuccessful = false;
                return result;
            }

            itinerary.Name = itineraryDto.Name;
            itinerary.Description = itineraryDto.Description;
            itinerary.IdUser = itineraryDto.IdUser;
            itinerary.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                result.Result = itinerary;
                return result;
            }
            catch (Exception ex)
            {
                result.ValidationMessage = ex.Message;
                result.IsSuccessful = false;
                return result;
            }
        }

        public async Task<ServiceResult<bool>> DeleteItineraryAsync(int id)
        {
            var result = new ServiceResult<bool>();
            var itinerary = await _context.Itineraries
                .Include(i => i.ItineraryDetails)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (itinerary == null)
            {
                result.ValidationMessage = "Itinerary not found";
                result.IsSuccessful = false;
                return result;
            }

            // Ștergem mai întâi detaliile
            _context.ItineraryDetails.RemoveRange(itinerary.ItineraryDetails);
            await _context.SaveChangesAsync();

            // Apoi ștergem itinerariul
            _context.Itineraries.Remove(itinerary);

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
                result.IsSuccessful = false;
                return result;
            }
        }

        public async Task<ServiceResult<bool>> DeleteItineraryByUser(int id, int userId)
        {
            var result = new ServiceResult<bool>();
            var itinerary = await _context.Itineraries
                .Include(i => i.ItineraryDetails)
                .FirstOrDefaultAsync(i => i.Id == id && i.IdUser == userId);

            if (itinerary == null)
            {
                result.ValidationMessage = "Itinerary not found";
                return result;
            }

            // Ștergem mai întâi detaliile
            _context.ItineraryDetails.RemoveRange(itinerary.ItineraryDetails);
            await _context.SaveChangesAsync();

            // Apoi ștergem itinerariul
            _context.Itineraries.Remove(itinerary);

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
                        .Include(x => x.ItineraryDetails)
                .ToListAsync();

            var itineraryPageDTOs = itineraries.Select(itinerary => new ItineraryPageDTO
            {
                Id = itinerary.Id,
                Name = itinerary.Name,
                Description = itinerary.Description,
                ItineraryDetails = itinerary.ItineraryDetails.Select(detail => new ItineraryDetailModel
                {
                    Id = detail.Id,
                    Name = detail.Name,
                    Descriere = detail.Descriere,
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



        public async Task<ServiceResult<List<ItineraryPageDTO>>> GetByUserId(int userId)
        {
            var result = new ServiceResult<List<ItineraryPageDTO>>();
            var itineraries = await _context.Itineraries
                .Include(i => i.ItineraryDetails)
                    .ThenInclude(i => i.Objective)
                        .ThenInclude(o => o.Images)
                .Include(i => i.ItineraryDetails)
                    .ThenInclude(i => i.Event)
                        .ThenInclude(e => e.Images)
                .Where(i => i.IdUser == userId || i.IdUser == null)
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
                    Name = detail.Name,
                    Descriere = detail.Descriere,
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
        public async Task<ServiceResult<ItineraryPageDTO>> GetByIdAsync(int id)
        {
            var result = new ServiceResult<ItineraryPageDTO>();

            var itinerary = await _context.Itineraries
                .Include(i => i.ItineraryDetails)
                    .ThenInclude(i => i.Objective)
                        .ThenInclude(o => o.Images)
                .Include(i => i.ItineraryDetails)
                    .ThenInclude(i => i.Event)
                        .ThenInclude(e => e.Images)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (itinerary == null)
            {
                result.ValidationMessage = "Itinerary not found";
                result.IsSuccessful = false;
                return result;
            }

            var itineraryPageDTO = new ItineraryPageDTO
            {
                Id = itinerary.Id,
                Name = itinerary.Name,
                Description = itinerary.Description,
                ItineraryDetails = itinerary.ItineraryDetails.Select(detail => new ItineraryDetailModel
                {
                    Id = detail.Id,
                    Name = detail.Name,
                    Descriere = detail.Descriere,
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
            };

            result.Result = itineraryPageDTO;
            result.IsSuccessful = true;
            return result;
        }

    }
}
