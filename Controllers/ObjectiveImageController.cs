using Microsoft.AspNetCore.Mvc;
using TravelSBE.Utils;

[Route("api/[controller]")]
[ApiController]
public class ObjectiveImageController : ControllerBase
{
    private readonly IObjectiveImageService _imageService;

    public ObjectiveImageController(IObjectiveImageService imageService)
    {
        _imageService = imageService;
    }

    // GET: api/ObjectiveImage/{objectiveId}
    [HttpGet("{objectiveId}")]
    public async Task<IActionResult> GetImages(int objectiveId)
    {
        var images = await _imageService.GetImagesByObjectiveIdAsync(objectiveId);
        return Ok(images.Select(img => new
        {
            img.Id,
            img.FilePath,
            img.ImageMimeType
        }));
    }

    [HttpPost("upload")]
    public async Task<ServiceResult<int>> UploadImage([FromForm] IFormFile imageFile, int? objectiveId=null, int? eventId = null,int? idExperience =null)
    {
        var result = await _imageService.UploadImageAsync(imageFile, objectiveId,eventId,idExperience);
        return result;
    }

    // DELETE: api/ObjectiveImage/delete/{imageId}
    [HttpDelete("delete")]
    public async Task<ServiceResult<bool>> DeleteImageAsync([FromQuery]string imageUrl)
    {
        var result = await _imageService.DeleteImageAsync(imageUrl);
        return result;
    }
}
