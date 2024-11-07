using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using tindog_marketplace;
using tindog_marketplace.DAL.Entities;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly JwtService _jwtService;
    private readonly UserManager<User> _userManager;

    public UserController(IUnitOfWork unitOfWork, JwtService jwtService, UserManager<User> userManager)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _userManager = userManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var claims = await _userManager.GetClaimsAsync(user);
                var accessToken = _jwtService.GenerateAccessToken(new ClaimsIdentity(claims));

                var refreshToken = _jwtService.GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                await _userManager.UpdateAsync(user);

                return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        return BadRequest(ModelState);
    }

    [Authorize]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] User model)
    {
        var user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        if (user == null || user.RefreshToken != model.RefreshToken)
            return Unauthorized();

        var claims = await _userManager.GetClaimsAsync(user);
        var newAccessToken = _jwtService.GenerateAccessToken(new ClaimsIdentity(claims));
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        // Update the user's refresh token in the database
        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        return Ok(new { AccessToken = newAccessToken, RefreshToken = newRefreshToken });
    }

    [Authorize]
    [HttpGet("GetAllUsers")]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUsersAsync()
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        return Ok(users);
    }

    [Authorize]
    [HttpGet("GetUserById/{id}", Name = "GetUserByIdName")]
    public async Task<ActionResult<User>> GetUserByIdAsync(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
            return NotFound();
        return Ok(user);
    }

    [Authorize]
    [HttpGet("GetUserWithOrders/{id}")]
    public async Task<ActionResult<UserWithOrders>> GetUserWithOrdersAsync(int id)
    {
        var userWithOrders = await _unitOfWork.Users.GetUserWithOrdersAsync(id);
        if (userWithOrders == null)
            return NotFound();
        return Ok(userWithOrders);
    }

    [Authorize]
    [HttpPost("AddUser")]
    public async Task<ActionResult> AddUserAsync([FromBody] User user)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var userId = await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.CompleteAsync();
        return CreatedAtRoute("GetUserByIdName", new { id = userId }, user);
    }

    [Authorize]
    [HttpPut("UpdateUser/{id}")]
    public async Task<ActionResult> UpdateUserAsync(int id, [FromBody] User user)
    {
        if (id != user.Id || !ModelState.IsValid)
            return BadRequest();

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.CompleteAsync();
        return NoContent();
    }

    [Authorize]
    [HttpDelete("DeleteUser/{id}")]
    public async Task<ActionResult> DeleteUserAsync(int id)
    {
        await _unitOfWork.Users.DeleteAsync(id);
        await _unitOfWork.CompleteAsync();
        return NoContent();
    }
}
