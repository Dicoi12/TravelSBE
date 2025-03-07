using TravelsBE.Entity;

namespace TravelsBE.Services.Interfaces
{
    public interface IObjectiveTypeService
    {
        Task<List<ObjectiveType>> GetAllAsync();
        Task<ObjectiveType?> GetByIdAsync(int id);
        Task<ObjectiveType> CreateAsync(ObjectiveType objectiveType);
        Task<bool> UpdateAsync(int id, ObjectiveType updatedObjectiveType);
        Task<bool> DeleteAsync(int id);
    }

}
