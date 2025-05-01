using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class MedicalRecordVM
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        [Required(ErrorMessage = "Disease name is required")]
        public string DiseaseName { get; set; }
        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
