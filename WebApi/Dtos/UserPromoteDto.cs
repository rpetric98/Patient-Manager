using System.ComponentModel.DataAnnotations;

namespace WebApi.Dtos
{
    public class UserPromoteDto
    {
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }
    }
}
