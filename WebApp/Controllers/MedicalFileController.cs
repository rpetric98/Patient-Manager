using ClassLibrary.DataContext;
using ClassLibrary.Interfaces;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class MedicalFileController : Controller
    {
        private readonly MedicalDbContext _context;
        private readonly IMinioService _minioService;
        public MedicalFileController(MedicalDbContext context, IMinioService minioService)
        {
            _context = context;
            _minioService = minioService;
        }


        //GET: MedicalFile
        public async Task<IActionResult> Index()
        {
            var medicalFiles = await _context.MedicalFiles
                .Select(m => new MedicalFileVM
                {
                    Id = m.Id,
                    FilePath = m.ObjectId,
                    ExaminationId = m.ExaminationId,
                }).ToListAsync();
            return View(medicalFiles);
        }

        //GET: MedicalFile/Details/X
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalFile = await _context.MedicalFiles
                .Select(m => new MedicalFileVM
                { 
                    Id = m.Id,
                    FilePath = m.ObjectId,
                    ExaminationId = m.ExaminationId,
                })
                .FirstOrDefaultAsync(m => m.Id == id);
            if (medicalFile == null)
            {
                return NotFound();
            }

            return View(medicalFile);
        }

        //GET: MedicalFile/Create
        public IActionResult Create(string filePath)
        {
            var model = new MedicalFileVM();
            if(!string.IsNullOrEmpty(filePath))
            {
                model.FilePath = filePath;
            }
           
            return View(model);
        }

        //POST: MedicalFile/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExaminationId,FilePath")] MedicalFileVM medicalFileVM)
        {
            if (ModelState.IsValid)
            {
                var newMedicalFile = new MedicalFile
                {
                    ExaminationId = medicalFileVM.ExaminationId,
                    ObjectId = medicalFileVM.FilePath
                };

                _context.Add(newMedicalFile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(medicalFileVM);
        }

        //GET: MedicalFile/Edit/X
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalFile = await _context.MedicalFiles
                .Select(m => new MedicalFileVM
                {
                    Id = m.Id,
                    FilePath = m.ObjectId,
                    ExaminationId = m.ExaminationId,
                })
                .FirstOrDefaultAsync(m => m.Id == id);
            if (medicalFile == null)
            {
                return NotFound();
            }
            return View(medicalFile);
        }

        //POST: MedicalFile/Edit/X
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ExaminationId,FilePath")] MedicalFileVM medicalFileVM, IFormFile newFile)
        {
            if (id != medicalFileVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingMedicalFile = await _context.MedicalFiles.FindAsync(id);
                    if (existingMedicalFile == null)
                    {
                        return NotFound();
                    }
                    if (newFile != null && newFile.Length > 0)
                    { 
                        await _minioService.DeleteObject(existingMedicalFile.ObjectId);

                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
                        var fileExtension = Path.GetExtension(newFile.FileName).ToLowerInvariant();
                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            return BadRequest("Invalid file type");
                        }

                        string newFilePath;
                        using (var stream = newFile.OpenReadStream())
                        {
                            newFilePath = await _minioService.PutObject(stream, newFile.FileName, newFile.ContentType, newFile.Length);
                        }    
                        existingMedicalFile.ObjectId = newFilePath;
                    }
                    existingMedicalFile.ExaminationId = medicalFileVM.ExaminationId;
                    _context.Update(existingMedicalFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.MedicalFiles.Any(e=> e.Id == id))
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
            return View(medicalFileVM);
        }

        //GET: MedicalFile/Delete/X
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalFile = await _context.MedicalFiles
                .Select(m => new MedicalFileVM
                {
                    Id = m.Id,
                    FilePath = m.ObjectId,
                    ExaminationId = m.ExaminationId,
                })
                .FirstOrDefaultAsync(m => m.Id == id);
            if (medicalFile == null)
            {
                return NotFound();
            }
            return View(medicalFile);
        }

        //POST: MedicalFile/Delete/X
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicalFile = await _context.MedicalFiles.FindAsync(id);
            if (medicalFile != null)
            {
                await _minioService.DeleteObject(medicalFile.ObjectId);
                _context.MedicalFiles.Remove(medicalFile);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        //POST: MedicalFile/UploadFile
        [HttpPost]
        [Route("MedicalFile/UploadFile")]
        public async Task<IActionResult> UploadFile(IFormFile formFile)
        {
            if (formFile == null || formFile.Length == 0)
            {
                return BadRequest("No file uploaded");
            }
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            var fileExtension = Path.GetExtension(formFile.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest("Invalid file type");
            }
            string filePath;
            using (var stream = formFile.OpenReadStream())
            {
                filePath = await _minioService.PutObject(stream, formFile.FileName, formFile.ContentType, formFile.Length);
            }
            return Json(new { filePath, fileName = formFile.FileName, size = formFile.Length });
        }

        //GET: MedicalFile/DownloadFile/X
        [HttpGet]
        [Route("MedicalFile/DownloadFile/{id}")]
        public async Task<IActionResult> DownloadFile(int id)
        {
            var medicalFile = await _context.MedicalFiles.FindAsync(id);
            if (medicalFile == null)
            {
                return NotFound();
            }
            var minioResponse = await _minioService.GetObjectResponse(medicalFile.ObjectId);
            if (minioResponse == null)
            {
                return NotFound();
            }
            return File(minioResponse.Data, minioResponse.ContentType, Path.GetFileName(medicalFile.ObjectId));
        }

    }
}
