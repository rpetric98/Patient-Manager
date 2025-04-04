using System.ComponentModel.DataAnnotations;

namespace WebApi.Dtos
{
    public class UserDto
    {
        public int UserId { get; set; }
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "Password should be at least 8 characters")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Firstname is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name should be between 2 and 50 characters")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Lastname is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name should be between 2 and 50 characters")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Provide a correct email address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Provide a correct phone number")]
        public string PhoneNumber { get; set; }
    }
}
