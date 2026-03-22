using TravelSBE.Entity;
using TravelSBE.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TravelSBE.Models;
using TravelsBE.Services.Interfaces;

namespace TravelSBE.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public ReviewService(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<Review>> GetReviewsByObjectiveId(int objectiveId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.IdObjective == objectiveId)
                .OrderByDescending(x=>x.CreatedAt)
                .ToListAsync();
        }

        public async Task<Review> AddReview(ReviewModel reviewDto)
        {
            var userId = _currentUserService.UserId ?? reviewDto.IdUser;

            var review = new Review
            {
                IdUser = userId,
                IdObjective = (int)reviewDto.IdObjective,
                Raiting = reviewDto.Raiting,
                Comment = reviewDto.Comment,
                DatePosted = reviewDto.DatePosted ?? DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
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