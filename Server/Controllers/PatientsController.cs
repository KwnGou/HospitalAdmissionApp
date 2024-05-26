using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalAdmissionApp.Server.Model;
using AutoMapper;
using HospitalAdmissionApp.Shared.DTOs;

namespace HospitalAdmissionApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public PatientsController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;  
        }

        // GET: api/Patients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient_GridDTO>>> GetPatients()
        {
          if (_context.Patients == null)
          {
              return NotFound();
          }
          //?
            List<Patient> result;
            result = await _context.Patients
                .Include(p => p.Disease)
                .ToListAsync();
            var mapped = _mapper.Map<IEnumerable<Patient_GridDTO>>(result);

            return Ok(mapped);  
        }

        // GET: api/Patients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Patient_DetailsDTO>> GetPatient(int id)
        {
          if (_context.Patients == null)
          {
              return NotFound();
          }
            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
            {
                return NotFound();
            }

            await _context.Entry(patient).Reference(p => p.Disease)
                .LoadAsync();

            var mapped = _mapper.Map<Patient_DetailsDTO>(patient);

            return Ok(mapped);
        }

        // PUT: api/Patients/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatient(int id, Patient_DetailsDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            //Data Validation
            var (res, msg) = await ValidateData(dto);
            if (!res)
            {
                return BadRequest(msg);
            }

            _context.Entry(dto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            //the application is not going to support multiple users the following exception is not needed at the moment 
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!DiseasExists(id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}
            catch (DbUpdateException ex)
            {
                return BadRequest($"{ex.Message}: {ex?.InnerException?.Message}");
            }
            return NoContent();
        }

        // POST: api/Patients
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Patient>> PostPatient(Patient_DetailsDTO dto)
        {
          if (_context.Patients == null)
          {
              return Problem("Entity set 'DataContext.Patients'  is null.");
          }
            var entity = _mapper.Map<Patient>(dto);

            //Data Validation
            var (res, msg) = await ValidateData(dto);
            if (!res)
            {
                return BadRequest(msg);
            }

            _context.Patients.Add(entity);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest($"{ex.Message}: {ex?.InnerException?.Message}");
            }

            await _context.Entry(entity).Reference(p => p.Disease)
                .LoadAsync();

            var mapped = _mapper.Map<Patient_DetailsDTO>(entity);

            return CreatedAtAction("GetPatient", new { id = mapped.Id }, mapped);
        }

        // DELETE: api/Patients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            if (_context.Patients == null)
            {
                return NotFound();
            }
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            _context.Patients.Remove(patient);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest($"{ex.Message}: {ex?.InnerException?.Message}");
            }

            return NoContent();
        }

        private bool PatientExists(int id)
        {
            return (_context.Patients?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private async Task<(bool result, string message)> ValidateData(Patient_DetailsDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return (false, "Patient name is required.");
            }

            dto.Name.Trim();

            if (string.IsNullOrWhiteSpace(dto.Surname))
            {
                return (false, "Patient surname is required.");
            }

            dto.Surname.Trim();

            if (string.IsNullOrWhiteSpace(dto.PatientIdentityCard))
            {
                return (false, "Patient PatientIdentityCard is required.");
            }

            dto.PatientIdentityCard.Trim();

            if (await _context.Patients.AnyAsync(p => p.PatientIdentityCard == dto.PatientIdentityCard))
            {
                return (false, "Specified Identity Card is already in use.");
            }

            return (true, string.Empty);
        }
    }
}
