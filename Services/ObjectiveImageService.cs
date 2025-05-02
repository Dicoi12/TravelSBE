using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TravelSBE.Data;
using TravelSBE.Entity;
using TravelSBE.Models;
using TravelSBE.Services.Interfaces;
using TravelSBE.Utils;

namespace TravelSBE.Services;

public class ObjectiveImageService : IObjectiveImageService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/images/");


    public ObjectiveImageService(ApplicationDbContext context)
    {
        _context = context;
        if (!Directory.Exists(_uploadPath))
            Directory.CreateDirectory(_uploadPath);
    }

    public async Task<List<ObjectiveImage>> GetImagesByObjectiveIdAsync(int objectiveId)
    {
        return await _context.ObjectiveImages
            .Where(img => img.IdObjective == objectiveId)
            .ToListAsync();
    }

    public async Task<ServiceResult<int>> UploadImageAsync(IFormFile imageFile, int? objectiveId, int? eventId = null, int? idExperience = null, int? idItinerary = null)
    {
        var result = new ServiceResult<int>();

        if (imageFile == null || imageFile.Length == 0)
        {
            result.ValidationMessage = "Invalid image file.";
            return result;
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
        if (!allowedExtensions.Contains(fileExtension))
        {
            result.ValidationMessage = "Unsupported file type.";
            return result;
        }

        var fileName = $"{Guid.NewGuid()}{fileExtension}";
        var filePath = Path.Combine(_uploadPath, fileName);

        try
        {
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            var objectiveImage = new ObjectiveImage
            {
                IdObjective = objectiveId ?? null,
                IdEvent = eventId,
                IdExperienta = idExperience,
                FilePath = $"/uploads/images/{fileName}",
                ImageMimeType = imageFile.ContentType
            };

            _context.ObjectiveImages.Add(objectiveImage);
            await _context.SaveChangesAsync();

            result.Result = objectiveImage.Id;
        }
        catch (Exception ex)
        {
            result.ValidationMessage = $"Error saving image: {ex.Message}";
        }
        result.IsSuccessful = true;
        return result;
    }


    public async Task<ServiceResult<bool>> DeleteImageAsync(string imageUrl)
    {
        var result = new ServiceResult<bool>();

        try
        {
            // Extract the file name from the URL
            var fileName = Path.GetFileName(new Uri(imageUrl).LocalPath);

            // Find the image in the database using the file name
            var image = await _context.ObjectiveImages
                .FirstOrDefaultAsync(x => x.FilePath.EndsWith(fileName));

            if (image == null)
            {
                result.ValidationMessage = "Image not found.";
                return result;
            }

            // Construct the full file path
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.FilePath.TrimStart('/'));

            // Delete the file from the disk if it exists
            if (File.Exists(filePath))
                File.Delete(filePath);

            // Remove the image record from the database
            _context.ObjectiveImages.Remove(image);
            await _context.SaveChangesAsync();

            result.Result = true;
            result.IsSuccessful = true;
        }
        catch (Exception ex)
        {
            result.ValidationMessage = $"Error deleting image: {ex.Message}";
            result.IsSuccessful = false;
        }

        return result;
    }


}