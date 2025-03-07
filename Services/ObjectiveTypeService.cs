using Microsoft.EntityFrameworkCore;
using System;
using TravelsBE.Entity;
using TravelsBE.Services.Interfaces;
using TravelSBE.Data;

namespace TravelsBE.Services
{
    public class ObjectiveTypeService : IObjectiveTypeService
    {
        private readonly ApplicationDbContext _context;

        public ObjectiveTypeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ObjectiveType>> GetAllAsync()
        {
            return await _context.ObjectiveTypes.ToListAsync();
        }

        public async Task<ObjectiveType?> GetByIdAsync(int id)
        {
            return await _context.ObjectiveTypes.FindAsync(id);
        }

        public async Task<ObjectiveType> CreateAsync(ObjectiveType objectiveType)
        {
            _context.ObjectiveTypes.Add(objectiveType);
            await _context.SaveChangesAsync();
            return objectiveType;
        }

        public async Task<bool> UpdateAsync(int id, ObjectiveType updatedObjectiveType)
        {
            var existingObjectiveType = await _context.ObjectiveTypes.FindAsync(id);
            if (existingObjectiveType == null)
                return false;

            existingObjectiveType.Name = updatedObjectiveType.Name;
            existingObjectiveType.Description = updatedObjectiveType.Description;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var objectiveType = await _context.ObjectiveTypes.FindAsync(id);
            if (objectiveType == null)
                return false;

            _context.ObjectiveTypes.Remove(objectiveType);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
