using ClassLibrary.DataContext;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class MedicalRecordController : Controller
    {
        private readonly MedicalDbContext _context;
        public MedicalRecordController(MedicalDbContext context)
        {
            _context = context;
        }

        //GET: MedicalRecord
        public async Task<IActionResult> Index()
        {
            var medicalReconds = await _context.MedicalRecords
                .Select(m => new MedicalRecordVM
                {
                    Id = m.Id,
                    PatientId = m.PatientId,
                    DiseaseName = m.Disease,
                    StartDate = m.StartDate,
                    EndDate = m.EndDate
                }).ToListAsync();

            return View(medicalReconds);
        }

        //GET: MedicalRecord/Create
        public IActionResult Create(int patientID)
        {
            var medicalRecordVM = new MedicalRecordVM {
                PatientId = patientID,
                StartDate = DateTime.UtcNow
            };

            return View(medicalRecordVM);
        }

        //POST: MedicalRecord/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicalRecordVM medicalRecordVM)
        {
            var patientExists = await _context.Patients.AnyAsync(p => p.Id == medicalRecordVM.PatientId);
            if (!patientExists)
            {
                ModelState.AddModelError("PatientId", "Patient does not exist.");
                return View(medicalRecordVM);
            }
            if (ModelState.IsValid)
            {
                var medicalRecord = new MedicalRecord
                {
                    PatientId = medicalRecordVM.PatientId,
                    Disease = medicalRecordVM.DiseaseName,
                    StartDate = medicalRecordVM.StartDate.ToUniversalTime(),
                    EndDate = medicalRecordVM.EndDate?.ToUniversalTime()
                };

                _context.Add(medicalRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(medicalRecordVM);
        }

        //GET: MedicalRecord/Edit/X
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalRecord = await _context.MedicalRecords.FindAsync(id);
            if (medicalRecord == null)
            {
                return NotFound();
            }

            var medicalRecordVM = new MedicalRecordVM
            {
                Id = medicalRecord.Id,
                PatientId = medicalRecord.PatientId,
                DiseaseName = medicalRecord.Disease,
                StartDate = medicalRecord.StartDate,
                EndDate = medicalRecord.EndDate
            };
            return View(medicalRecordVM);
        }

        //POST: MedicalRecord/Edit/X
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MedicalRecordVM medicalRecordVM)
        {
            if (id != medicalRecordVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var patientExists = await _context.Patients.AnyAsync(p => p.Id == medicalRecordVM.PatientId);
                if(!patientExists)
                {
                    ModelState.AddModelError("PatientId", "Patient does not exist.");
                    return View(medicalRecordVM);
                }

                try
                {
                    var medicalRecord = await _context.MedicalRecords.FindAsync(id);
                    if (medicalRecord == null)
                    {
                        return NotFound();
                    }
                    medicalRecord.PatientId = medicalRecordVM.PatientId;
                    medicalRecord.Disease = medicalRecordVM.DiseaseName;
                    medicalRecord.StartDate = medicalRecordVM.StartDate.ToUniversalTime();
                    medicalRecord.EndDate = medicalRecordVM.EndDate?.ToUniversalTime();

                    _context.Update(medicalRecord);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.MedicalRecords.Any(m => m.Id == id))
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
            return View(medicalRecordVM);
        }

        //GET: MedicalRecord/Delete/X
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalRecord = await _context.MedicalRecords
                .Select(m => new MedicalRecordVM
                {
                    Id = m.Id,
                    PatientId = m.PatientId,
                    DiseaseName = m.Disease,
                    StartDate = m.StartDate,
                    EndDate = m.EndDate
                }).FirstOrDefaultAsync(m => m.Id == id);
            if (medicalRecord == null)
            {
                return NotFound();
            }
            return View(medicalRecord);
        }

        //POST: MedicalRecord/Delete/X
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicalRecord = await _context.MedicalRecords.FindAsync(id);
            if (medicalRecord != null)
            {
                _context.MedicalRecords.Remove(medicalRecord);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        //GET: MedicalRecord/Details/X
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalRecord = await _context.MedicalRecords
                .Select(m => new MedicalRecordVM
                {
                    Id = m.Id,
                    PatientId = m.PatientId,
                    DiseaseName = m.Disease,
                    StartDate = m.StartDate,
                    EndDate = m.EndDate
                }).FirstOrDefaultAsync(m => m.Id == id);
            if (medicalRecord == null)
            {
                return NotFound();
            }
            return View(medicalRecord);
        }
    }
}
