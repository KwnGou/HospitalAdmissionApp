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
using HospitalAdmissionApp.Client.Pages;
using Humanizer;
using Microsoft.Data.SqlClient;

namespace HospitalAdmissionApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BedsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public BedsController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Beds
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bed_GridDTO>>> GetBeds([FromQuery] int? roomId)
        {
          if (_context.Beds == null)
          {
              return NotFound();
            }

            List<Bed> result;

            if (roomId.HasValue)
            {
                result = await _context.Beds
                    .Where(b => b.RoomId == roomId.Value)
                    .Include(r => r.Room)
                    .ToListAsync();
            }
            else
            {
                result = await _context.Beds
                    .Include(b => b.Room)
                    .ToListAsync();
            }

            var mapped = _mapper.Map<IEnumerable<Bed_GridDTO>>(result);

            return Ok(mapped);
        }

        // GET: api/AvailableBedsForPatient
        [HttpGet("AvailableBedsForPatient", Name = nameof(AvailableBedsForPatient))]
        public async Task<ActionResult<IEnumerable<Bed_GridDTO>>> AvailableBedsForPatient(
            [FromQuery(Name = "p")] int? patientId,
            [FromQuery(Name = "d")] int? diseaseId,
            [FromQuery(Name = "downgrade")] bool? downgrade)
        {
            if (_context.Beds == null)
            {
                return NotFound();
            }
            if (!patientId.HasValue || !diseaseId.HasValue)
            {
                return BadRequest("Patient and Disease ID are required");
            }
            var patient = await _context.Patients.FindAsync(patientId.Value);
            if (patient == null)
            {
                return NotFound($"Patient with Id {patientId.Value} not found");
            }
            var disease = await _context.Diseases.FindAsync(diseaseId.Value);
            if (disease == null)
            {
                return NotFound($"Disease with Id {diseaseId.Value} not found");
            }

            List<object> parameters = new() { 
                new SqlParameter("PatientInsurance", patient.Insurance),
                new SqlParameter("PatientDisease", disease.Id)};

            var cmdTxt = string.Empty;
            if (downgrade.HasValue && downgrade.Value) 
            {
                cmdTxt = @"  
SELECT 
	DISTINCT(B1.Id) AS BedId, B1.BedInfo AS BedInfo,
	R1.Id AS RoomId, R1.RoomNumber AS RoomNumber 
FROM ( 
	SELECT B.RoomId AS RoomId 
	FROM Beds B
	GROUP By B.RoomId
	HAVING COUNT(B.RoomId) >= @PatientInsurance) B2
INNER JOIN Beds B1 ON B1.RoomId = B2.RoomId
INNER JOIN Rooms R1 ON B1.RoomId = R1.Id
INNER JOIN Clinics C1 ON R1.ClinicId = C1.Id
INNER JOIN Diseases D1 ON D1.ClinicId = C1.Id
LEFT OUTER JOIN Slots S1 ON S1.BedId = B1.Id
WHERE 
	(D1.Id = @PatientDisease AND D1.ClinicId = R1.ClinicId AND R1.Id = B1.RoomId AND S1.AdmissionDate IS NULL)  
	OR B1.Id IN (SELECT S2.BedId FROM Slots S2 JOIN Patients P2 ON S2.PatientId = P2.Id WHERE S2.ReleaseDate IS NOT NULL)
";
            }
            else // not present or false
            {
                cmdTxt = @"  
SELECT 
	DISTINCT(B1.Id) AS BedId, B1.BedInfo AS BedInfo,
	R1.Id AS RoomId, R1.RoomNumber AS RoomNumber 
FROM ( 
	SELECT B.RoomId AS RoomId 
	FROM Beds B
	GROUP By B.RoomId
	HAVING COUNT(B.RoomId) = @PatientInsurance) B2
INNER JOIN Beds B1 ON B1.RoomId = B2.RoomId
INNER JOIN Rooms R1 ON B1.RoomId = R1.Id
INNER JOIN Clinics C1 ON R1.ClinicId = C1.Id
INNER JOIN Diseases D1 ON D1.ClinicId = C1.Id
LEFT OUTER JOIN Slots S1 ON S1.BedId = B1.Id
WHERE 
	(D1.Id = @PatientDisease AND D1.ClinicId = R1.ClinicId AND R1.Id = B1.RoomId AND S1.AdmissionDate IS NULL)  
	OR B1.Id IN (SELECT S2.BedId FROM Slots S2 JOIN Patients P2 ON S2.PatientId = P2.Id WHERE S2.ReleaseDate IS NOT NULL)";
            }

            try
            {
                var result = await _context.SelectableBeds.FromSqlRaw(cmdTxt, parameters.ToArray()).ToListAsync();
                return Ok(result);
            }
            catch (Exception ex)
            { 
                return BadRequest($"{ex.Message} : {ex?.InnerException?.Message}");  
            }
        }

        // GET: api/Beds/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Bed_GridDTO>> GetBed(int id)
        {
          if (_context.Beds == null)
          {
              return NotFound();
          }
            var bed = await _context.Beds.FindAsync(id);

            if (bed == null)
            {
                return NotFound();
            }

            await _context.Entry(bed).Reference(b => b.Room).LoadAsync();

            var mapped = _mapper.Map<Bed_GridDTO>(bed);

            return Ok(mapped);
        }

        // PUT: api/Beds/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBed(int id, Bed_GridDTO dto)
        {
            if (id != dto.BedId)
            {
                return BadRequest();
            }

            //Data Validation
            var (res, msg) = await ValidateData(dto);
            if (!res)
            {
                return BadRequest(msg);
            }

            var entity = _mapper.Map<Bed>(dto);

            _context.Entry(entity).State = EntityState.Modified;

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

        // POST: api/Beds
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Bed>> PostBed(Bed_GridDTO dto)
        {
          if (_context.Beds == null)
          {
              return Problem("Entity set 'DataContext.Beds'  is null.");
          }
            var entity = _mapper.Map<Bed>(dto);

            //Data Validation
            var (res, msg) = await ValidateData(dto);
            if (!res)
            {
                return BadRequest(msg);
            }

            _context.Beds.Add(entity);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest($"{ex.Message}: {ex?.InnerException?.Message}");
            }

            await _context.Entry(entity).Reference(b => b.Room).LoadAsync();

            var mapped = _mapper.Map<Bed_GridDTO>(entity);

            return CreatedAtAction("GetBed", new { id = mapped.BedId }, mapped);
        }

        // DELETE: api/Beds/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBed(int id)
        {
            if (_context.Beds == null)
            {
                return NotFound();
            }
            var bed = await _context.Beds.FindAsync(id);
            if (bed == null)
            {
                return NotFound();
            }

            _context.Beds.Remove(bed);
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

        private bool BedExists(int id)
        {
            return (_context.Beds?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private async Task<(bool result, string message)> ValidateData(Bed_GridDTO dto)
        {
            if (dto.RoomId == null)
            {
                return (false, "RoomId is required.");
            }

            return (true, string.Empty);
        }
    }
}
