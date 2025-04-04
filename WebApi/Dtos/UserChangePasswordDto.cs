using System.ComponentModel.DataAnnotations;

namespace WebApi.Dtos
{
    public class UserChangePasswordDto
    {
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Passwoord is required")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "Password should be at least 8 characters")]
        public string Password { get; set; }
    }
}
