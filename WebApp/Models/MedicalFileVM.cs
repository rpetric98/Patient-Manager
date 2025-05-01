using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class MedicalFileVM
    {
        public int Id { get; set; }
        public int ExaminationId { get; set; }
        [Required(ErrorMessage = "File path is required")]
        public string FilePath { get; set; }
    }
}
