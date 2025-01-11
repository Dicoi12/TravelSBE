using System.Collections.Generic;
using System.Threading.Tasks;
using TravelsBE.Entity;
using TravelsBE.Models;
using TravelSBE.Utils;

namespace TravelsBE.Services.Interfaces
{
    public interface IExperienceService
    {
        Task<ServiceResult<List<ExperienceModel>>> GetAllExperiencesAsync();
        Task<ServiceResult<List<ExperienceModel>>> GetExperienceByCityOrCoords(string? city, double? lat, double? lon);
        Task<ServiceResult<ExperienceModel>> GetExperienceByIdAsync(int id);
        Task<ServiceResult<ExperienceModel>> AddExperience(ExperienceModel request);
        Task<ServiceResult<ExperienceModel>> UpdateExperience(ExperienceModel request);
        Task<ServiceResult<bool>> DeleteExperience(int id);
    }
}
