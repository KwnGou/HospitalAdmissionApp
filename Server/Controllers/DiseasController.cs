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
    public class DiseasController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public DiseasController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Diseas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Disease_GridDTO>>> GetDiseases([FromQuery] int? clinicId)
        {
            if (_context.Diseases == null)
            {
                return NotFound();
            }
            List<Diseas> result;

            if (clinicId.HasValue)
            {
                result = await _context.Diseases
                    .Where(d => d.ClinicId == clinicId.Value)
                    .Include(c => c.Clinic)
                    .ToListAsync();
            }
            else
            {
                result = await _context.Diseases
                    .Include(d => d.Clinic)
                    .ToListAsync();
            }

            var mapped = _mapper.Map<IEnumerable<Disease_GridDTO>>(result);

            return Ok(mapped);
        }

        // GET: api/Diseas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Disease_GridDTO>> GetDiseas(int id)
        {
            if (_context.Diseases == null)
            {
                return NotFound();
            }
            var diseas = await _context.Diseases.FindAsync(id);

            if (diseas == null)
            {
                return NotFound();
            }

            await _context.Entry(diseas).Reference(d => d.Clinic).LoadAsync();

            var mapped = _mapper.Map<Disease_GridDTO>(diseas);

            return Ok(mapped);
        }

        // PUT: api/Diseas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDiseas(int id, Disease_GridDTO dto)
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

            var entity = _mapper.Map<Diseas>(dto);

            _context.Entry(entity).State = EntityState.Modified;

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

        // POST: api/Diseas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Disease_GridDTO>> PostDiseas(Disease_GridDTO dto)
        {
            if (_context.Diseases == null)
            {
                return Problem("Entity set 'DataContext.Diseases'  is null.");
            }

            var entity = _mapper.Map<Diseas>(dto);

            //Data Validation
            var (res, msg) = await ValidateData(dto);
            if (!res)
            {
                return BadRequest(msg);
            }

            _context.Diseases.Add(entity);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest($"{ex.Message}: {ex?.InnerException?.Message}");
            }

            await _context.Entry(entity).Reference(d => d.Clinic).LoadAsync();
            
            var mapped = _mapper.Map<Disease_GridDTO>(entity);

            return CreatedAtAction("GetDiseas", new { id = mapped.Id }, mapped);
        }

        // DELETE: api/Diseas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiseas(int id)
        {
            if (_context.Diseases == null)
            {
                return NotFound();
            }
            var diseas = await _context.Diseases.FindAsync(id);
            if (diseas == null)
            {
                return NotFound();
            }
            if (await _context.Patients.AllAsync(p => p.DiseaseId == id)) 
            {
                return BadRequest("There is a patient with the specific disease.");
            }

            _context.Diseases.Remove(diseas);

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

        private bool DiseasExists(int id)
        {
            return (_context.Diseases?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private async Task<(bool result, string message)> ValidateData(Disease_GridDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return (false, "Disease name is required.");
            }

            dto.Name.Trim();

            if (await _context.Diseases.AnyAsync(d => d.Name == dto.Name))
            {
                return (false, "Specified disease name already exist.");
            }

            if (!(await _context.Clinics.AnyAsync(c => c.Id == dto.ClinicId)))
            {
                return (false, "Specified clinic does not exist.");
            }

            return (true, string.Empty);
        }
    }
}
