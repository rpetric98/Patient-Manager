using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class UserLoginVM
    {
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "Password should be at least 8 chars long")]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}
