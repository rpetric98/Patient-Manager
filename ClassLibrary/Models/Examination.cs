using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    public class Examination
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public int PatientId { get; set; }
        public virtual ICollection<MedicalFile> MedicalFiles { get; set; }
        public virtual Patient Patient { get; set; }
    }
}
