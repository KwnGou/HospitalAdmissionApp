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
using Microsoft.AspNetCore.Http.HttpResults;
using NuGet.Protocol;

namespace HospitalAdmissionApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SlotsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public SlotsController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Slots
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Slot_GridDTO>>> GetSlots([FromQuery] int? patientId, int? bedId)
        {
            if (_context.Slots == null)
            {
                return NotFound();
            }
            List<Slot> result;
            if (patientId.HasValue)
            {
                result = await _context.Slots
                    .Where(s => s.PatientId == patientId.Value)
                    .Where(s => s.BedId == bedId.Value)
                    .Include(s => s.Patient)
                    .Include(s => s.Bed)
                    .ToListAsync();
            }
            else
            {
                result = await _context.Slots
                .Include(s => s.Patient)
                .Include(s => s.Bed)
                    .ToListAsync();
            }

            var mapped = _mapper.Map<IEnumerable<Slot_GridDTO>>(result);

            return Ok(mapped);
        }

        // GET: api/Slots/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Slot_GridDTO>> GetSlot(int id)
        {
            if (_context.Slots == null)
            {
                return NotFound();
            }
            var slot = await _context.Slots.FindAsync(id);

            if (slot == null)
            {
                return NotFound();
            }

            await _context.Entry(slot).Reference(s => s.Patient)
                .LoadAsync();
            await _context.Entry(slot).Reference(s => s.Bed)
               .LoadAsync();

            var mapped = _mapper.Map<Slot_GridDTO>(slot);

            return Ok(mapped);
        }

        // GET: api/Slots/patientId
        [HttpGet("patientId")]
        public async Task<ActionResult<Slot_GridDTO>> GetPatientSlot(int id)
        {
            var result = await _context.Slots.Where(s => s.PatientId == id).FirstAsync();

            return Ok(result);
        }

        // PUT: api/Slots/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSlot(int id, Slot_GridDTO dto)
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

        // POST: api/Slots
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Slot>> PostSlot(Slot_GridDTO dto)
        {
            if (_context.Slots == null)
            {
                return Problem("Entity set 'DataContext.Slots'  is null.");
            }
            var entity = _mapper.Map<Slot>(dto);

            //Data Validation
            var (res, msg) = await ValidateData(dto);
            if (!res)
            {
                return BadRequest(msg);
            }

            _context.Slots.Add(entity);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest($"{ex.Message}: {ex?.InnerException?.Message}");
            }

            await _context.Entry(entity).Reference(s => s.Patient)
                .LoadAsync();
            await _context.Entry(entity).Reference(s => s.Bed)
               .LoadAsync();

            var mapped = _mapper.Map<Slot_GridDTO>(entity);

            return CreatedAtAction("GetDiseas", new { id = mapped.Id }, mapped);
        }

        // DELETE: api/Slots/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSlot(int id)
        {
            if (_context.Slots == null)
            {
                return NotFound();
            }
            var slot = await _context.Slots.FindAsync(id);
            if (slot == null)
            {
                return NotFound();
            }

            _context.Slots.Remove(slot);

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

        private bool SlotExists(int id)
        {
            return (_context.Slots?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private async Task<(bool result, string message)> ValidateData(Slot_GridDTO dto)
        {
            if (dto.AdmissionDate < DateTimeOffset.Now.Date)
            {
                return (false, "Admission date cannot be before today.");
            }
            if (dto.ReleaseDate < DateTimeOffset.Now.Date)
            {
                return (false, "Release date cannot be before today.");
            }

            if (!(await _context.Patients.AnyAsync(p => p.Id == dto.PatientId)))
            {
                return (false, "Specified patient id does not exist.");
            }

            if (!(await _context.Beds.AnyAsync(b => b.Id == dto.BedId)))
            {
                return (false, "Specified bed id does not exist.");
            }

            return (true, string.Empty);
        }
    }
}
