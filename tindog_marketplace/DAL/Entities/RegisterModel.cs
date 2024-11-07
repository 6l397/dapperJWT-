using System.ComponentModel.DataAnnotations;

namespace tindog_marketplace.DAL.Entities
{
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Паролі не співпадають")]
        public string ConfirmPassword { get; set; }
    }
}
