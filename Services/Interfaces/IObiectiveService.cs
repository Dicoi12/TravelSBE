using System.Collections.Generic;
using System.Threading.Tasks;
using TravelsBE.Models.Filters;
using TravelSBE.Entity;
using TravelSBE.Models;
using TravelSBE.Utils;

namespace TravelSBE.Services.Interfaces
{
    public interface IObjectiveService
    {
        Task<ServiceResult<List<ObjectiveModel>>> GetObjectivesAsync(string? search);
        Task<ServiceResult<List<ObjectiveModel>>> GetLocalObjectives(ObjectiveFilterModel filter);
        Task<ServiceResult<ObjectiveModel>> GetObjectiveByIdAsync(int id);
        Task<ServiceResult<ObjectiveModel>> CreateObjectiveAsync(ObjectiveModel objective);
        Task<ServiceResult<ObjectiveModel>> UpdateObjectiveAsync(Objective objective);
        Task<ServiceResult<bool>> DeleteObjectiveAsync(int id);
    }
}
