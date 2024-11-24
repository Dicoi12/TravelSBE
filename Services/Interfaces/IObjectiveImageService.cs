using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using TravelSBE.Entity;
using TravelSBE.Utils;

public interface IObjectiveImageService
{
    Task<List<ObjectiveImage>> GetImagesByObjectiveIdAsync(int objectiveId);
    Task<ServiceResult<int>> UploadImageAsync(IFormFile imageFile, int? objectiveId, int? eventId = null);
    Task<ServiceResult<bool>> DeleteImageAsync(int imageId);
}
