using TravelSBE.Models;
using TravelSBE.Utils;

namespace TravelSBE.Services.Interfaces
{
    public interface IObjectiveImageService
    {
        Task<ServiceResult<ObjectiveImageModel>> GetImageByIdAsync(int id);
        Task<ServiceResult<List<ObjectiveImageModel>>> GetImagesByObjectiveIdAsync(int objectiveId);
        Task<ServiceResult<ObjectiveImageModel>> AddImageAsync(ObjectiveImageModel imageModel);
        Task<ServiceResult<bool>> DeleteImageAsync(int id);
    }
}
