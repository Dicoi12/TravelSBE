using TravelSBE.Entity;
using TravelSBE.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TravelSBE.Models;

namespace TravelSBE.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Review>> GetReviewsByObjectiveId(int objectiveId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.IdObjective == objectiveId)
                .ToListAsync();
        }

        public async Task<Review> AddReview(ReviewModel reviewDto)
        {
            var review = new Review
            {
                IdUser = reviewDto.IdUser,
                IdObjective = reviewDto.IdObjective,
                Raiting = reviewDto.Raiting,
                Comment = reviewDto.Comment,
                DatePosted = reviewDto.DatePosted ?? DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<Review> GetReviewById(int id)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<bool> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return false;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}