using System.Collections.Generic;
using System.Threading.Tasks;
using TravelSBE.Entity;
using TravelSBE.Models;
using TravelSBE.Utils;

namespace TravelSBE.Services.Interfaces
{
    public interface IObjectiveService
    {
        Task<ServiceResult<List<ObjectiveModel>>> GetObjectivesAsync();
        Task<ServiceResult<List<ObjectiveModel>>> GetLocalObjectives(double latitude,double longitude);
        Task<ServiceResult<ObjectiveModel>> GetObjectiveByIdAsync(int id);
        Task<ServiceResult<ObjectiveModel>> CreateObjectiveAsync(ObjectiveModel objective);
        Task<ServiceResult<ObjectiveModel>> UpdateObjectiveAsync(Objective objective);
        Task<ServiceResult<bool>> DeleteObjectiveAsync(int id);
    }
}
