using ClassLibrary.Enums;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class ExaminationVM
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        [Required(ErrorMessage = "Examination type is required")]
        public ExaminationType ExaminationType { get; set; }
        [Required(ErrorMessage = "Date time is required")]
        public DateTime DateTime { get; set; }
    }
}
