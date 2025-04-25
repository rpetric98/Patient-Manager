using ClassLibrary.Models;
using ClassLibrary.RepoFactory;
using ClassLibrary.Utilities;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IRepository<Patient> _repository;
        public PatientController(IRepository<Patient> repository)
        {
            _repository = repository;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetAllPatients()
        {
            var patients = await _repository.GetAllAsync();
            var patientDtos = patients.Select(x => new PatientDto
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Gender = x.Gender,
                OIB = x.OIB,
                DateOfBirth = x.DateOfBirth
            }).ToList();

            return Ok(patientDtos);
        }

        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<PatientDto>> GetPatientById(int id)
        { 
            var patient = await _repository.GetByIdAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            var patientDto = new PatientDto
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Gender = patient.Gender,
                OIB = patient.OIB,
                DateOfBirth = patient.DateOfBirth
            };
            return Ok(patientDto);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<PatientDto>> CreatePatient(PatientDto patientDto)
        {
            //TODO
            if (!OIBValidator.IsValidOIB(patientDto.OIB))
            {
                return BadRequest("Invalid OIB format.");
            }

            var patient = new Patient
            {
                FirstName = patientDto.FirstName,
                LastName = patientDto.LastName,
                Gender = patientDto.Gender,
                OIB = patientDto.OIB,
                DateOfBirth = patientDto.DateOfBirth
            };

            await _repository.AddAsync(patient);

            patientDto.Id = patient.Id;

            return CreatedAtAction(nameof(GetPatientById), new { id = patient.Id }, patientDto);
        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdatePatient(int id, PatientDto patientDto)
        {
            if (id != patientDto.Id)
            {
                return BadRequest();
            }

            if (!OIBValidator.IsValidOIB(patientDto.OIB))
            {
                return BadRequest("Invalid OIB format.");
            }

            var patient = new Patient
            { 
                Id = patientDto.Id,
                FirstName = patientDto.FirstName,
                LastName = patientDto.LastName,
                Gender = patientDto.Gender,
                OIB = patientDto.OIB,
                DateOfBirth = patientDto.DateOfBirth
            };

            await _repository.UpdateAsync(patient);
            return NoContent();
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patient = await _repository.GetByIdAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(patient.Id);
            return NoContent();
        }
    }
}
