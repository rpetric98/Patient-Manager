using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class PatientVM
    {
        public int PatientId { get; set; }
        [Required(ErrorMessage = "Firstname is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Lastname is required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }
        [Required(ErrorMessage = "OIB is required")]
        public string OIB { get; set; }
        [Required(ErrorMessage = "Date of birth is required")]
        public DateTime DateOfBirth { get; set; }
        public ICollection<ExaminationVM> Examinations { get; set; } = new List<ExaminationVM>();
        public ICollection<MedicalRecordVM> MedicalRecords { get; set; } = new List<MedicalRecordVM>();
        public ICollection<PrescriptionVM> Prescriptions { get; set; } = new List<PrescriptionVM>();
    }
}
