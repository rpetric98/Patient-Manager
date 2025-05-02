using ClassLibrary.DataContext;
using ClassLibrary.Dictionaries;
using ClassLibrary.Enums;
using ClassLibrary.Models;
using ClassLibrary.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class PatientController : Controller
    {
        private readonly MedicalDbContext _context;
        private readonly CSVExporter _csvExporter;

        public PatientController(MedicalDbContext context, CSVExporter csvExporter)
        {
            _context = context;
            _csvExporter = csvExporter;
        }


        //GET: Patient
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            IQueryable<Patient> patients = _context.Patients;

            if (!String.IsNullOrEmpty(searchString))
            {
                patients = patients.Where(p => p.OIB.Contains(searchString) || p.LastName.Contains(searchString));
            }

            var patientList = await patients.ToListAsync();

            var patientVM = patientList
                .Select(p => new PatientVM
                { 
                    PatientId = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    OIB = p.OIB,
                    Gender = p.Gender,
                    DateOfBirth = p.DateOfBirth,
                }).ToList();


            return View(patientVM);
        }

        //GET: Patient/Create
        public IActionResult Create()
        {
            return View();
        }

        //POST: Patient/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PatientVM patient)
        {
            if (ModelState.IsValid)
            {
                var newPatient = new Patient
                {
                    FirstName = patient.FirstName,
                    LastName = patient.LastName,
                    OIB = patient.OIB,
                    Gender = patient.Gender,
                    DateOfBirth = DateTime.SpecifyKind(patient.DateOfBirth, DateTimeKind.Utc)
                };

                _context.Add(newPatient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }

        //GET: Patient/Edit/X
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Select(p => new PatientVM
                { 
                    PatientId = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    OIB = p.OIB,
                    Gender = p.Gender,
                    DateOfBirth = p.DateOfBirth,
                }).FirstOrDefaultAsync(p => p.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        //POST: Patient/Edit/X
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PatientVM patient)
        {
            if (id != patient.PatientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingPatient = await _context.Patients.FindAsync(id);
                    if (existingPatient == null)
                    {
                        return NotFound();
                    }
                    existingPatient.FirstName = patient.FirstName;
                    existingPatient.LastName = patient.LastName;
                    existingPatient.OIB = patient.OIB;
                    existingPatient.Gender = patient.Gender;
                    existingPatient.DateOfBirth = DateTime.SpecifyKind(patient.DateOfBirth, DateTimeKind.Utc);
                    _context.Update(existingPatient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Patients.Any(p => p.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }

        //GET: Patient/Delete/X
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Select(p => new PatientVM
                { 
                    PatientId = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    OIB = p.OIB,
                    Gender = p.Gender,
                    DateOfBirth = p.DateOfBirth,
                }).FirstOrDefaultAsync(p => p.PatientId == id);

            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        //POST: Patient/Delete/X
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        //GET: Patient/Export
        public async Task<IActionResult> ExportToCSV(int id)
        { 
            var patient = await _context.Patients
                .Include(p => p.Examinations)
                .Include(p => p.MedicalRecords)
                .Include(p => p.Prescriptions)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null)
            {
                return NotFound();
            }
            var csv = _csvExporter.ExportPatientsToCSV(new List<Patient> { patient});
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", $"{patient.OIB}_Export.csv");
        }

        //GET: Patient/Details/X
        public IActionResult Details(int id)
        { 
            var patient = _context.Patients
                .Where(p => p.Id == id)
                .Select(p => new PatientVM
                { 
                    PatientId = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    OIB = p.OIB,
                    Gender = p.Gender,
                    DateOfBirth = p.DateOfBirth,
                    Examinations = p.Examinations.Select(e => new ExaminationVM
                    { 
                        DateTime = e.Date,
                        ExaminationType = Enum.Parse<ExaminationType>(e.Type)
                    }).ToList(),
                    MedicalRecords = p.MedicalRecords.Select(m => new MedicalRecordVM
                    {
                        DiseaseName = m.Disease,
                        StartDate = m.StartDate,
                        EndDate = m.EndDate,
                    }).ToList(),
                    Prescriptions = p.Prescriptions.Select(pr => new PrescriptionVM
                    {
                        MedicineName = pr.Medication,
                        Dosage = pr.Dosage,
                    }).ToList()
                }).FirstOrDefault();

            if (patient == null)
            {
                return NotFound();
            }

            ViewBag.ExaminationTypes = ExaminationTypeDict.Description;

            return View(patient);
        }
    }
}
