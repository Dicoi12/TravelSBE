using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TravelSBE.Data;
using TravelSBE.Entity;
using TravelSBE.Models;
using TravelSBE.Services.Interfaces;
using TravelSBE.Utils;

namespace TravelSBE.Services
{
    public class ObjectiveImageService : IObjectiveImageService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ObjectiveImageService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResult<ObjectiveImageModel>> GetImageByIdAsync(int id)
        {
            ServiceResult<ObjectiveImageModel> result = new();

            var imageEntity = await _context.ObjectiveImages.FindAsync(id);
            if (imageEntity == null)
            {
                result.ValidationMessage = "Image not found.";
                return result;
            }

            result.Result = _mapper.Map<ObjectiveImageModel>(imageEntity);
            return result;
        }

        public async Task<ServiceResult<List<ObjectiveImageModel>>> GetImagesByObjectiveIdAsync(int objectiveId)
        {
            ServiceResult<List<ObjectiveImageModel>> result = new();

            var imageEntities = await _context.ObjectiveImages
                .Where(img => img.IdObjective == objectiveId)
                .ToListAsync();

            result.Result = _mapper.Map<List<ObjectiveImageModel>>(imageEntities);
            return result;
        }

        public async Task<ServiceResult<ObjectiveImageModel>> AddImageAsync(ObjectiveImageModel imageModel)
        {
            ServiceResult<ObjectiveImageModel> result = new();

            if (imageModel != null)
            {
                var imageEntity = _mapper.Map<ObjectiveImage>(imageModel);
                await _context.ObjectiveImages.AddAsync(imageEntity);
                await _context.SaveChangesAsync();
                result.Result = _mapper.Map<ObjectiveImageModel>(imageEntity);
            }
            else
            {
                result.ValidationMessage = "Invalid image data.";
            }

            return result;
        }

        public async Task<ServiceResult<bool>> DeleteImageAsync(int id)
        {
            ServiceResult<bool> result = new();

            var imageEntity = await _context.ObjectiveImages.FindAsync(id);
            if (imageEntity != null)
            {
                _context.ObjectiveImages.Remove(imageEntity);
                await _context.SaveChangesAsync();
                result.Result = true;
            }
            else
            {
                result.ValidationMessage = "Image not found.";
                result.Result = false;
            }

            return result;
        }
    }
}
