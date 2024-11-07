using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using tindog_marketplace.DAL.Entities;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public ReviewController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }


    [HttpGet]
    public async Task<IActionResult> GetAllReviews()
    {
        var reviews = await _unitOfWork.Reviews.GetAllAsync();
        return Ok(reviews);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetReviewById(int id)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id);
        if (review == null)
            return NotFound();
        return Ok(review);
    }


    [HttpPost]
    public async Task<IActionResult> AddReview([FromBody] Review review)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var newReviewId = await _unitOfWork.Reviews.AddAsync(review);
        await _unitOfWork.CompleteAsync();

        return CreatedAtAction(nameof(GetReviewById), new { id = newReviewId }, review);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReview(int id, [FromBody] Review review)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingReview = await _unitOfWork.Reviews.GetByIdAsync(id);
        if (existingReview == null)
            return NotFound();

        await _unitOfWork.Reviews.UpdateAsync(review);
        await _unitOfWork.CompleteAsync();

        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var existingReview = await _unitOfWork.Reviews.GetByIdAsync(id);
        if (existingReview == null)
            return NotFound();

        await _unitOfWork.Reviews.DeleteAsync(id);
        await _unitOfWork.CompleteAsync();

        return NoContent();
    }


    [HttpGet("by-seller/{sellerId}")]
    public async Task<IActionResult> GetReviewsBySeller(int sellerId)
    {
        var reviews = await _unitOfWork.Reviews.GetReviewsBySellerIdWithUserAsync(sellerId);
        return Ok(reviews);
    }
}