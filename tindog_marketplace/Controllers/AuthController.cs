using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;
using tindog_marketplace.DAL.Entities;

namespace tindog_marketplace.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly UserManager<User> _userManager;

        public AuthController(JwtService jwtService, UserManager<User> userManager)
        {
            _jwtService = jwtService;
            _userManager = userManager;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized();

            var claims = await _userManager.GetClaimsAsync(user);
            var accessToken = _jwtService.GenerateAccessToken(new ClaimsIdentity(claims));

            
            var refreshToken = _jwtService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            await _userManager.UpdateAsync(user); 

            return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] User model)
        {
            var user = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (user == null || user.RefreshToken != model.RefreshToken)
                return Unauthorized();

            
            var claims = await _userManager.GetClaimsAsync(user);
            var newAccessToken = _jwtService.GenerateAccessToken(new ClaimsIdentity(claims));
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            
            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);

            return Ok(new { AccessToken = newAccessToken, RefreshToken = newRefreshToken });
        }
    }
}
