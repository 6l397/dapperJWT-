using Microsoft.AspNetCore.Mvc;
using tindog_marketplace.DAL.Entities;


[ApiController]
[Route("api/[controller]")]
public class SellerController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public SellerController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("GetAllSellers")]
    public async Task<ActionResult<IEnumerable<Seller>>> GetAllSellersAsync()
    {
        var sellers = await _unitOfWork.Sellers.GetAllAsync();
        return Ok(sellers);
    }

    [HttpGet("GetSellerById/{id}", Name = "GetSellerByIdName")]
    public async Task<ActionResult<Seller>> GetSellerByIdAsync(int id)
    {
        var seller = await _unitOfWork.Sellers.GetByIdAsync(id);
        if (seller == null)
            return NotFound();
        return Ok(seller);
    }

    [HttpGet("GetTopSellersWithReviews/{topCount}")]
    public async Task<ActionResult<IEnumerable<SellerWithReviews>>> GetTopSellersWithReviewsAsync(int topCount)
    {
        var topSellers = await _unitOfWork.Sellers.GetTopSellersWithReviewsAsync(topCount);
        return Ok(topSellers);
    }

    [HttpPost("AddSeller")]
    public async Task<ActionResult> AddSellerAsync([FromBody] Seller seller)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var sellerId = await _unitOfWork.Sellers.AddAsync(seller);
        await _unitOfWork.CompleteAsync();
        return CreatedAtRoute("GetSellerByIdName", new { id = sellerId }, seller);
    }

    [HttpPut("UpdateSeller/{id}")]
    public async Task<ActionResult> UpdateSellerAsync(int id, [FromBody] Seller seller)
    {
        if (id != seller.Id || !ModelState.IsValid)
            return BadRequest();

        await _unitOfWork.Sellers.UpdateAsync(seller);
        await _unitOfWork.CompleteAsync();
        return NoContent();
    }

    [HttpDelete("DeleteSeller/{id}")]
    public async Task<ActionResult> DeleteSellerAsync(int id)
    {
        await _unitOfWork.Sellers.DeleteAsync(id);
        await _unitOfWork.CompleteAsync();
        return NoContent();
    }
}
