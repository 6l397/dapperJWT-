using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using tindog_marketplace.DAL.Entities;

namespace tindog_marketplace
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;

        public JwtMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                
                using (var scope = _serviceProvider.CreateScope())
                {
                    var jwtService = scope.ServiceProvider.GetRequiredService<JwtService>();
                    var principal = jwtService.GetPrincipalFromExpiredToken(token);

                    if (principal != null)
                    {
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                        var userId = principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                        var user = await userManager.FindByIdAsync(userId);

                        if (user != null)
                        {
                            
                            context.Items["User"] = user;
                            context.User = principal; 
                        }
                    }
                }
            }

            await _next(context);
        }
    }

}
