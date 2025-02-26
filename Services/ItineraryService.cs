using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TravelsBE.Models;
using TravelsBE.Services.Interfaces;
using TravelSBE.Data;
using TravelSBE.Entity;
using TravelSBE.Models;
using TravelSBE.Utils;

namespace TravelsBE.Services
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
        public async Task<ServiceResult<List<ItineraryModel>>> GetAllAsync()
        {
            var result = new ServiceResult<List<ItineraryModel>>();
            try
            {
                var list = await _context.Itineraries.ToListAsync();
                result.IsSuccessful = true;
                var mapped = _mapper.Map<List<ItineraryModel>>(list);
                result.Result = mapped;
                return result;
            }
            catch (Exception ex)
            {
                result.ValidationMessage = ex.Message;
                return result;
            }
        }

        public async Task<ServiceResult<List<ItineraryModel>>> GetByUserId(int userId)
        {
            var result = new ServiceResult<List<ItineraryModel>>();
            try
            {
                var list = await _context.Itineraries.Where(x => x.IdUser == userId).ToListAsync();
                result.IsSuccessful = true;
                var mapped = _mapper.Map<List<ItineraryModel>>(list);
                result.Result = mapped;
                return result;
            }
            catch (Exception ex)
            {
                result.ValidationMessage = ex.Message;
                return result;
            }
        }

        public async Task<ServiceResult<ItineraryModel>> AddItineraryByUser(ItineraryModel model)
        {
            ServiceResult<ItineraryModel> result = new();
            try
            {
                var mapped = _mapper.Map<Itinerary>(model);
                _context.Add(mapped);
                await _context.SaveChangesAsync();
                result.Result = model;
                return result;
            }
            catch (Exception ex)
            {
                result.ValidationMessage = ex.Message;
                return result;
            }
        }
        public async Task<ServiceResult<ItineraryModel>> AddItinerary(ItineraryDTO model)
        {
            ServiceResult<ItineraryModel> result = new();
            try
            {
                if (!(model.EventsIds.Any() && model.ObjectivesIds.Any()))
                {
                    result.ValidationMessage = "Nu ati selectat niciun obiectiv sau eveniment";
                    return result;
                }
                if (string.IsNullOrEmpty(model.Name))
                {
                    result.ValidationMessage = "Nu ati ales denumirea itinerariului!";
                    return result;
                }
                Itinerary it = new()
                {
                    Name = model.Name,
                    IdUser = model.IdUser,
                };
                _context.Add(it);
                await _context.SaveChangesAsync();
                List<ItineraryDetail> idList = new();
                foreach (var obj in model.ObjectivesIds)
                {
                    ItineraryDetail id = new()
                    {
                        Name = "",
                        IdObjective = obj,
                        IdItinerary = it.Id,
                    };
                    idList.Add(id);
                }
                foreach (var ev in model.EventsIds)
                {
                    ItineraryDetail id = new()
                    {
                        Name = "",
                        IdEvent = ev,
                        IdItinerary = it.Id,
                    };
                    idList.Add(id);
                }
                _context.AddRange(idList);
                await _context.SaveChangesAsync();
                result.Result = new ItineraryModel();
                return result;
            }
            catch (Exception ex)
            {
                result.ValidationMessage = ex.Message;
                return result;
            }
        }

        public async Task<ServiceResult<bool>> DeleteItineraryByUser(int id, int userId)
        {
            ServiceResult<bool> result = new();
            try
            {
                var entityToRemove = await _context.Itineraries.FirstOrDefaultAsync(s => s.IdUser == userId && s.Id == id);
                if (entityToRemove != null)
                {
                    _context.Remove(entityToRemove);
                    await _context.SaveChangesAsync();
                    result.Result = true;
                    return result;
                }
                else
                {
                    result.IsSuccessful = false;
                    result.Result = false;
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.ValidationMessage = ex.Message;
                return result;
            }
        }

        public async Task<ServiceResult<ItineraryModel>> EditItineraryByUser(ItineraryModel model)
        {
            ServiceResult<ItineraryModel> result = new();
            var entity = await _context.Itineraries.FirstOrDefaultAsync(x => x.Id == model.Id);
            entity.Name = model.Name;
            entity.IdUser = model.Id;
            _context.Update(entity);
            await _context.SaveChangesAsync();
            result.Result = _mapper.Map<ItineraryModel>(entity);
            return result;
        }

    }
}
