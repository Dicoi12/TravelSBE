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
        Task<ObjectiveModel> CreateObjectiveAsync(ObjectiveModel objective);
        Task<ObjectiveModel> UpdateObjectiveAsync(ObjectiveModel objective);
        Task<bool> DeleteObjectiveAsync(int id);
    }
}
