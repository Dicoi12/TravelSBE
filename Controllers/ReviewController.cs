using Microsoft.AspNetCore.Mvc;
using TravelSBE.Services;
using TravelSBE.Entity;
using TravelSBE.Models;

namespace TravelSBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("objective/{objectiveId}")]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviewsByObjectiveId(int objectiveId)
        {
            var reviews = await _reviewService.GetReviewsByObjectiveId(objectiveId);
            return Ok(reviews);
        }

        [HttpPost]
        public async Task<ActionResult<Review>> AddReview(ReviewModel review)
        {
            var createdReview = await _reviewService.AddReview(review);
            return CreatedAtAction(nameof(GetReviewById), new { id = createdReview.Id }, createdReview);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> GetReviewById(int id)
        {
            var review = await _reviewService.GetReviewById(id);
            if (review == null) return NotFound();
            return Ok(review);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var success = await _reviewService.DeleteReview(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}