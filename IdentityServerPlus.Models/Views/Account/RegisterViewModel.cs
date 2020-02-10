using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models.Views
{
    public class RegisterViewModel : AuthenticateViewModel
    {
        [Required]
        [StringLength(50, ErrorMessage = "Must be between 2 and 50 characters", MinimumLength = 2)]
        [Display(Name = "Username")]
        public override string Username { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(128, ErrorMessage = "Must be between 6 and 128 characters", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public override string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }


        public RegisterViewModel()
        {

        }

        public RegisterViewModel(AuthenticateViewModel model) : base(model)
        {
            
        }
    }
}