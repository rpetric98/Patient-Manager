using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class PrescriptionVM
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        [Required(ErrorMessage = "Medication is required")]
        public string MedicineName { get; set; }
        [Required(ErrorMessage = "Dosage is required")]
        public string Dosage { get; set; }
    }
}
