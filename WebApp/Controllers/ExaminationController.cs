using ClassLibrary.DataContext;
using ClassLibrary.Dictionaries;
using ClassLibrary.Enums;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class ExaminationController : Controller
    {
        private readonly MedicalDbContext _context;
        public ExaminationController(MedicalDbContext context)
        {
            _context = context;
        }

        //GET: Examination
        public async Task<IActionResult> Index()
        {
            var examinations = await _context.Examinations.ToListAsync();
            return View(examinations);
        }

        //GET: Examination/Details/X
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var examination = await _context.Examinations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (examination == null)
            {
                return NotFound();
            }

            return View(examination);
        }

        //GET: Examination/Create
        public IActionResult Create(int? patientId)
        { 
            var examinationVM = new ExaminationVM
            {
                DateTime = DateTime.UtcNow
            };
            if (patientId != null)
            {
                examinationVM.PatientId = patientId.Value;
            }
            ViewBag.ExaminationTypes = ExaminationTypeDict.Description;
            return View(examinationVM);
        }

        //POST: Examination/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExaminationVM examinationVM)
        {
            if (ModelState.IsValid)
            {
                examinationVM.DateTime = DateTime.SpecifyKind(examinationVM.DateTime, DateTimeKind.Utc);
                var patientExists = await _context.Patients.AnyAsync(p => p.Id == examinationVM.PatientId);
                if (!patientExists)
                {
                    ModelState.AddModelError("PatientId", "The selected patient does not exist.");
                    ViewBag.ExaminationTypes = ExaminationTypeDict.Description;
                    return View(examinationVM);
                }
                var examination = new Examination
                {
                    Date = examinationVM.DateTime,
                    Type = examinationVM.ExaminationType.ToString(),
                    PatientId = examinationVM.PatientId,
                };
                _context.Add(examination);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ExaminationTypes = ExaminationTypeDict.Description;
            return View(examinationVM);
        }

        //GET: Examination/Edit/X
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var examination = await _context.Examinations.FindAsync(id);
            if (examination == null)
            {
                return NotFound();
            }

            var examinationVM = new ExaminationVM
            { 
                Id = examination.Id,
                DateTime = examination.Date,
                ExaminationType = Enum.Parse<ExaminationType>(examination.Type),
                PatientId = examination.PatientId
            };

            ViewBag.ExaminationTypes = ExaminationTypeDict.Description;
            return View(examinationVM);
        }

        //POST: Examination/Edit/X
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ExaminationVM examinationVM)
        {
            if (id != examinationVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var patientExists = await _context.Patients.AnyAsync(p => p.Id == examinationVM.PatientId);
                if (!patientExists)
                {
                    ModelState.AddModelError("PatientId", "The selected patient does not exist.");
                    ViewBag.ExaminationTypes = ExaminationTypeDict.Description;
                    return View(examinationVM);
                }
                try
                {
                 
                    var existingExamination = await _context.Examinations.FindAsync(id);
                    if (existingExamination == null)
                    {
                        return NotFound();
                    }

                    examinationVM.DateTime = DateTime.SpecifyKind(examinationVM.DateTime, DateTimeKind.Utc);
                    existingExamination.Date = examinationVM.DateTime;
                    existingExamination.Type = examinationVM.ExaminationType.ToString();
                    existingExamination.PatientId = examinationVM.PatientId;

                    _context.Update(existingExamination);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Examinations.Any(e => e.Id == id))
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
            ViewBag.ExaminationTypes = ExaminationTypeDict.Description;
            return View(examinationVM);
        }

        //GET: Examination/Delete/X
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var examination = await _context.Examinations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (examination == null)
            {
                return NotFound();
            }

            return View(examination);
        }

        //POST: Examination/Delete/X
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var examination = await _context.Examinations.FindAsync(id);
            if (examination != null)
            {
                _context.Examinations.Remove(examination);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
