using ClassLibrary.DataContext;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.RepoFactory
{
    public class PatientRepository : IRepository<Patient>
    {
        private readonly MedicalDbContext _context;
        public PatientRepository(MedicalDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Patient entity)
        {
            await _context.Patients.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Patient entity)
        {
            _context.Patients.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await _context.Patients.ToListAsync();
        }

        public async Task<Patient?> GetByIdAsync(int id)
        { 
            return await _context.Patients
                .Include(p => p.Prescriptions)
                .Include(e => e.Examinations)
                .Include(mr => mr.MedicalRecords)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

    }
}
