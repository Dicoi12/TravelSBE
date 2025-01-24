using TravelSBE.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;
using TravelSBE.Models;

namespace TravelSBE.Services
{
    public interface IReviewService
    {
        Task<IEnumerable<Review>> GetReviewsByObjectiveId(int objectiveId);
        Task<Review> AddReview(ReviewModel review);
        Task<Review> GetReviewById(int id);
        Task<bool> DeleteReview(int id);
    }
}