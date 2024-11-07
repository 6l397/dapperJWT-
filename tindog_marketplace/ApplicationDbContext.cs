using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using tindog_marketplace.DAL.Entities;

namespace tindog_marketplace
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

       
    }
}
