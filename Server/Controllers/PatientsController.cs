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
using HospitalAdmissionApp.Shared;
using Microsoft.Build.Framework;
using HospitalAdmissionApp.Client.Pages;
using static System.Reflection.Metadata.BlobBuilder;

namespace HospitalAdmissionApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public PatientsController(DataContext context, IMapper mapper, IConfiguration config)
        {
            _context = context;
            _mapper = mapper;
            _config = config;
        }

        // GET: api/Patients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient_GridDTO>>> GetPatients()
        {
            if (_context.Patients == null)
            {
                return NotFound();
            }
            List<Patient> result;
            result = await _context.Patients
                .ToListAsync();
            var mapped = _mapper.Map<IEnumerable<Patient_GridDTO>>(result);

            return Ok(mapped);
        }
        // GET: api/patientsForAdmission
        [HttpGet("patientsForAdmission", Name = nameof(GetPatientsForAdmission))]
        public async Task<ActionResult<IEnumerable<PatientSelection_DTO>>> GetPatientsForAdmission()
        {
            if (_context.Patients == null)
            {
                return NotFound();
            }


            var cmdTxt = @"
SELECT P1.Id AS Id, P1.[Name] AS [Name], P1.Surname AS Surname, P1.PatientIdentityCard AS PatientIdentityCard
FROM Patients P1
WHERE P1.Id NOT IN (SELECT Slots.PatientId FROM Slots)
	UNION
SELECT P2.Id AS Id, P2.[Name] AS [Name], P2.Surname AS Surname, P2.PatientIdentityCard AS PatientIdentityCard
FROM Patients P2
JOIN Slots
ON P2.Id = Slots.PatientId
WHERE NOT Slots.ReleaseDate IS NULL
";

            try
            {
                var result = await _context.SelectablePatients.FromSqlRaw(cmdTxt).ToArrayAsync();
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}: {ex?.InnerException?.Message}");
            }
        }

        // GET: api/Patients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Patient_DetailsDTO>> GetPatient(int id)
        {
            if (_context.Patients == null)
            {
                return NotFound();
            }
            var result = await _context.Patients.FindAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            var mapped = _mapper.Map<Patient_DetailsDTO>(result);

            // fix other visual data
            var sexOptions = _config.GetSection(EnumOption.SexOptionsString).Get<EnumOption[]>();
            var insuranceOptions = _config.GetSection(EnumOption.InsuranceOptionsString).Get<EnumOption[]>();

            mapped.SexText = sexOptions.First(s => s.Id == mapped.Sex).Text;
            mapped.InsuranceText = insuranceOptions.First(i => i.Id == mapped.Insurance).Text;
            mapped.Age = DateTime.Now.Year - mapped.DateOfBirth.Year;

            return Ok(mapped);
        }

        // PUT: api/Patients/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatient(int id, Patient_EditDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            //Data Validation
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return Problem("Patient name is required.");
            }

            dto.Name.Trim();

            if (string.IsNullOrWhiteSpace(dto.Surname))
            {
                return Problem("Patient surname is required.");
            }

            dto.Surname.Trim();

            if (string.IsNullOrWhiteSpace(dto.PatientIdentityCard))
            {
                return Problem("Patient PatientIdentityCard is required.");
            }

            dto.PatientIdentityCard.Trim();

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
        public async Task<ActionResult<Patient>> PostPatient(Patient_EditDTO dto)
        {
            if (_context.Patients == null)
            {
                return Problem("Entity set 'DataContext.Patients'  is null.");
            }
            var entity = _mapper.Map<Patient>(dto);

            //Data Validation
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return Problem("Patient name is required.");
            }

            dto.Name.Trim();

            if (string.IsNullOrWhiteSpace(dto.Surname))
            {
                return Problem("Patient surname is required.");
            }

            dto.Surname.Trim();

            if (string.IsNullOrWhiteSpace(dto.PatientIdentityCard))
            {
                return Problem("Patient PatientIdentityCard is required.");
            }

            dto.PatientIdentityCard.Trim();
            // propably have to modify this if we proceed with the memory feature
            if (await _context.Patients.AnyAsync(p => p.PatientIdentityCard == dto.PatientIdentityCard))
            {
                return Problem("Specified Identity Card is already in use.");
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

            if (_context.Slots.Any(s => s.PatientId == id))
            {
                return BadRequest("Patient in use");
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

    }
}
