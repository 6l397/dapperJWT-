using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using tindog_marketplace.DAL.Entities;

[Route("api/[controller]")]
[ApiController]
public class MarketplaceOrderController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public MarketplaceOrderController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await _unitOfWork.Orders.GetAllAsync();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(id);
        if (order == null)
            return NotFound();
        return Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> AddOrder([FromBody] MarketplaceOrder order)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var newOrderId = await _unitOfWork.Orders.AddAsync(order);
        await _unitOfWork.CompleteAsync();

        return CreatedAtAction(nameof(GetOrderById), new { id = newOrderId }, order);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, [FromBody] MarketplaceOrder order)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingOrder = await _unitOfWork.Orders.GetByIdAsync(id);
        if (existingOrder == null)
            return NotFound();

        await _unitOfWork.Orders.UpdateAsync(order);
        await _unitOfWork.CompleteAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var existingOrder = await _unitOfWork.Orders.GetByIdAsync(id);
        if (existingOrder == null)
            return NotFound();

        await _unitOfWork.Orders.DeleteAsync(id);
        await _unitOfWork.CompleteAsync();

        return NoContent();
    }


    [HttpGet("by-seller/{sellerId}")]
    public async Task<IActionResult> GetOrdersBySeller(int sellerId)
    {
        var orders = await _unitOfWork.Orders.GetOrdersByUserIdWithSellerAsync(sellerId);
        return Ok(orders);
    }
}
