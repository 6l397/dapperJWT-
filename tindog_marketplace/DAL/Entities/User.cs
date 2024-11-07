using Microsoft.AspNetCore.Identity;

namespace tindog_marketplace.DAL.Entities
{
    public class User : IdentityUser<int>
    {
        public string RefreshToken { get; set; }
    }

}
