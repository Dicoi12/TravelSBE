using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TravelSBE.Data;
using TravelSBE.Entity;
using TravelSBE.Models;
using TravelsBE.Services.Interfaces;
using TravelSBE.Utils;

namespace TravelsBE.Services
{
    public class ItineraryDetailService : IItineraryDetailService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ItineraryDetailService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResult<ItineraryDetail>> AddItineraryDetailAsync(ItineraryDetailModel itineraryDetailDto)
        {
            var result = new ServiceResult<ItineraryDetail>();

            var detail = new ItineraryDetail
            {
                Name = itineraryDetailDto.Name,
                Descriere = itineraryDetailDto.Descriere,
                IdObjective = itineraryDetailDto.IdObjective,
                IdEvent = itineraryDetailDto.IdEvent,
                VisitOrder = itineraryDetailDto.VisitOrder,
                IdItinerary = itineraryDetailDto.IdItinerary,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.ItineraryDetails.Add(detail);
            await _context.SaveChangesAsync();

            result.Result = detail;
            return result;
        }

        public async Task<ServiceResult<ItineraryDetail>> UpdateItineraryDetailAsync(ItineraryDetailModel itineraryDetailDto)
        {
            var result = new ServiceResult<ItineraryDetail>();

            var detail = await _context.ItineraryDetails.FindAsync(itineraryDetailDto.Id);
            if (detail == null)
            {
                result.ValidationMessage = "ItineraryDetail not found";
                result.IsSuccessful = false;
                return result;
            }

            detail.Name = itineraryDetailDto.Name;
            detail.Descriere = itineraryDetailDto.Descriere;
            detail.IdObjective = itineraryDetailDto.IdObjective;
            detail.IdEvent = itineraryDetailDto.IdEvent;
            detail.VisitOrder = itineraryDetailDto.VisitOrder;
            detail.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            result.Result = detail;
            return result;
        }

        public async Task<ServiceResult<bool>> DeleteItineraryDetailAsync(int id)
        {
            var result = new ServiceResult<bool>();

            var detail = await _context.ItineraryDetails.FindAsync(id);
            if (detail == null)
            {
                result.ValidationMessage = "ItineraryDetail not found";
                result.IsSuccessful = false;
                return result;
            }

            _context.ItineraryDetails.Remove(detail);
            await _context.SaveChangesAsync();

            result.IsSuccessful = true;
            result.Result = true;
            return result;
        }

        public async Task<ServiceResult<ItineraryDetail>> GetItineraryDetailByIdAsync(int id)
        {
            var result = new ServiceResult<ItineraryDetail>();

            var detail = await _context.ItineraryDetails.FindAsync(id);
            if (detail == null)
            {
                result.ValidationMessage = "ItineraryDetail not found";
                result.IsSuccessful = false;
                return result;
            }

            result.Result = detail;
            return result;
        }

        public async Task<ServiceResult<List<ItineraryDetail>>> GetAllItineraryDetailsAsync()
        {
            var result = new ServiceResult<List<ItineraryDetail>>();

            var details = await _context.ItineraryDetails.ToListAsync();
            result.Result = details;
            return result;
        }
    }
}

