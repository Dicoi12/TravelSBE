using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelSBE.Data;
using TravelSBE.Entity;
using TravelSBE.Models;
using TravelSBE.Services.Interfaces;
using TravelSBE.Utils;

namespace TravelSBE.Services
{
    public class ObjectiveService : IObjectiveService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public ObjectiveService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResult<List<ObjectiveModel>>> GetObjectivesAsync()
        {
            var result = new ServiceResult<List<ObjectiveModel>>();
            var list = await _context.Objectives.ToListAsync();
            result.Result = _mapper.Map<List<ObjectiveModel>>(list);
            return result;
        }

        public async Task<ServiceResult<ObjectiveModel>> GetObjectiveByIdAsync(int id)
        {
            ServiceResult<ObjectiveModel> result = new();
            var item = await _context.Objectives.Where(x => x.Id == id).FirstOrDefaultAsync();
            result.Result = _mapper.Map<ObjectiveModel>(item);
            return result;
        }

        public async Task<ServiceResult<ObjectiveModel>> CreateObjectiveAsync(ObjectiveModel objective)
        {
            ServiceResult<ObjectiveModel> result = new();
            objective.CreatedAt = DateTime.UtcNow;
            objective.UpdatedAt = DateTime.UtcNow;
            var mapped = _mapper.Map<Objective>(objective);
            _context.Objectives.Add(mapped);
            await _context.SaveChangesAsync();
            result.Result = _mapper.Map<ObjectiveModel>(mapped);
            return result;
        }

        public async Task<ServiceResult<ObjectiveModel>> UpdateObjectiveAsync(ObjectiveModel objective)
        {
            ServiceResult<ObjectiveModel> result = new();
            if (!_context.Objectives.Any(e => e.Id == objective.Id))
            {
                return new();
            }

            objective.UpdatedAt = DateTime.UtcNow;
            _context.Entry(objective).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            result.Result = _mapper.Map<ObjectiveModel>(objective);
            return result;
        }

        public async Task<ServiceResult<bool>> DeleteObjectiveAsync(int id)
        {
            ServiceResult<bool> result = new();
            var objective = await _context.Objectives.FindAsync(id);
            if (objective == null)
            {
                result.Result= false;
            }

            _context.Objectives.Remove(objective);
            await _context.SaveChangesAsync();
            result.Result= true;
            return result;
        }
    }
}
