using ClassLibrary.DataContext;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class PrescriptionController : Controller
    {
        private readonly MedicalDbContext _context;
        public PrescriptionController(MedicalDbContext context)
        {
            _context = context;
        }

        //GET: Prescription
        public async Task<IActionResult> Index()
        {
            var prescriptions = await _context.Prescriptions
                .Select(p => new PrescriptionVM
                { 
                   Id = p.Id,
                   PatientId = p.PatientId,
                   MedicineName = p.Medication,
                   Dosage = p.Dosage,
                })
                .ToListAsync();

            return View(prescriptions);
        }

        //GET: Prescription/Create
        public IActionResult Create(int? patientID)
        {
            var prescriptionVM = new PrescriptionVM();
            if (patientID != null)
            {
                prescriptionVM.PatientId = patientID.Value;
            }
            return View(prescriptionVM);
        }

        //POST: Prescription/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PrescriptionVM prescriptionVM)
        {
            if (ModelState.IsValid)
            {
                var prescription = new Prescription
                {
                    PatientId = prescriptionVM.PatientId,
                    Medication = prescriptionVM.MedicineName,
                    Dosage = prescriptionVM.Dosage,
                };

                _context.Prescriptions.Add(prescription);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(prescriptionVM);
        }

        //GET: Prescription/Edit
        public async Task<IActionResult> Edit(int id)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null)
            {
                return NotFound();
            }

            var prescriptionVM = new PrescriptionVM
            {
                Id = prescription.Id,
                PatientId = prescription.PatientId,
                MedicineName = prescription.Medication,
                Dosage = prescription.Dosage,
            };
            return View(prescriptionVM);
        }

        //POST: Prescription/Edit/X
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,PrescriptionVM prescriptionVM)
        {
            if (id != prescriptionVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var prescription = await _context.Prescriptions.FindAsync(id);
                if (prescription == null)
                {
                    return NotFound();
                }

                prescription.PatientId = prescriptionVM.PatientId;
                prescription.Medication = prescriptionVM.MedicineName;
                prescription.Dosage = prescriptionVM.Dosage;


                _context.Update(prescription);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(prescriptionVM);
        }

        //GET: Prescription/Delete/X
        public async Task<IActionResult> Delete(int id)
        {
            var prescription = await _context.Prescriptions
                .Where(p => p.Id == id)
                .Select(p => new PrescriptionVM
                {
                    Id = p.Id,
                    PatientId = p.PatientId,
                    MedicineName = p.Medication,
                    Dosage = p.Dosage,
                })
                .FirstOrDefaultAsync();
               
            if (prescription == null)
            {
                return NotFound();
            }

            return View(prescription);
        }

        //POST: Prescription/Delete/X
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null)
            {
                return NotFound();
            }

            _context.Prescriptions.Remove(prescription);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //GET: Prescription/Details/X
        public async Task<IActionResult> Details(int id)
        {
            var prescription = await _context.Prescriptions
                .Where(p => p.Id == id)
                .Select(p => new PrescriptionVM
                {
                    Id = p.Id,
                    PatientId = p.PatientId,
                    MedicineName = p.Medication,
                    Dosage = p.Dosage,
                })
                .FirstOrDefaultAsync();

            if (prescription == null)
            {
                return NotFound();
            }

            return View(prescription);
        }
    }
}
