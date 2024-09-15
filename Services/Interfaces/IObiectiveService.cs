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
        Task<ServiceResult<ObjectiveModel>> GetObjectiveByIdAsync(int id);
        Task<ServiceResult<ObjectiveModel>> CreateObjectiveAsync(ObjectiveModel objective);
        Task<ServiceResult<ObjectiveModel>> UpdateObjectiveAsync(ObjectiveModel objective);
        Task<ServiceResult<bool>> DeleteObjectiveAsync(int id);
    }
}
