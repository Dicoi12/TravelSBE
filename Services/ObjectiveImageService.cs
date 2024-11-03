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
            Directory.CreateDirectory(_uploadPath); // Crează directorul dacă nu există
    }

    // Metodă pentru obținerea tuturor imaginilor asociate unui obiectiv
    public async Task<List<ObjectiveImage>> GetImagesByObjectiveIdAsync(int objectiveId)
    {
        return await _context.ObjectiveImages
            .Where(img => img.IdObjective == objectiveId)
            .ToListAsync();
    }

    // Metodă pentru încărcarea unei imagini asociate unui obiectiv
    public async Task<ServiceResult<int>> UploadImageAsync(IFormFile imageFile, int objectiveId, int? eventId = null)
    {
        var result = new ServiceResult<int>();

        // Crează un nume unic pentru fișier și obține calea completă
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
        var filePath = Path.Combine(_uploadPath, fileName);

        // Salvează fișierul pe disc
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        // Salvează datele imaginii în baza de date
        var objectiveImage = new ObjectiveImage
        {
            IdObjective = objectiveId,
            IdEvent = eventId,
            FilePath = $"/uploads/images/{fileName}",
            ImageMimeType = imageFile.ContentType,
            ImageData = new byte[12]
        };

        _context.ObjectiveImages.Add(objectiveImage);
        await _context.SaveChangesAsync();

        result.Result = objectiveImage.Id;
        return result;
    }

    // Metodă pentru ștergerea unei imagini
    public async Task<ServiceResult<bool>> DeleteImageAsync(int imageId)
    {
        var result = new ServiceResult<bool>();

        // Găsește imaginea după ID
        var image = await _context.ObjectiveImages.FindAsync(imageId);
        if (image == null)
        {
            result.ValidationMessage=("Image not found.");
            return result;
        }

        // Șterge fișierul de pe disc
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.FilePath.TrimStart('/'));
        if (File.Exists(filePath))
            File.Delete(filePath);

        // Șterge înregistrarea din baza de date
        _context.ObjectiveImages.Remove(image);
        await _context.SaveChangesAsync();

        result.Result = true;
        return result;
    }
}