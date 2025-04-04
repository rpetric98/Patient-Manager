using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    public class Prescription
    {
        public int Id { get; set; }
        public string Medication { get; set; }
        public string Dosage { get; set; }
        public virtual Patient Patient { get; set; }
        public int PatientId { get; set; }
    }
}
