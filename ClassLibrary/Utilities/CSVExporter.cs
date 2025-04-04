using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Utilities
{
    public class CSVExporter
    {
        public string ExportPatientsToCSV(IEnumerable<Patient> patients)
        {
            if (patients == null || !patients.Any())
            {
                throw new ArgumentException("No patients to export.");
            }

            var csv = new StringBuilder();
            csv.AppendLine("Id,FirstName,LastName,Gender,OIB,DateOfBirth,Examinations,MedicalRecords,Prescriptions");

            foreach (var patient in patients)
            {
                var medicalRecords = string.Join(";", patient.MedicalRecords.Select(mr => $"{mr.Disease} ({mr.StartDate.ToShortDateString()} - {mr.EndDate?.ToShortDateString() ?? "Present"})"));
                var examinations = string.Join(";", patient.Examinations.Select(ex => $"{ex.Type} ({ex.Date.ToShortDateString()})"));
                var prescriptions = string.Join(";", patient.Prescriptions.Select(pr => $"{pr.Medication} ({pr.Dosage})"));

                csv.AppendLine($"{patient.Id},{patient.FirstName},{patient.LastName},{patient.OIB},{patient.DateOfBirth.ToShortDateString()},{patient.Gender},{medicalRecords},{examinations},{prescriptions}");
            }

            return csv.ToString();
        }

    }
}
