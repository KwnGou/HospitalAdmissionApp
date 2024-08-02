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
        public async Task<ActionResult<IEnumerable<Slot_GridDTO>>> GetSlots([FromQuery] int? patientId)
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
                    .Include(s => s.Patient)
                    .Include(s => s.Disease)
                    .Include(s => s.Bed)
                    .ThenInclude(b => b.Room)
                    .ToListAsync();
            }
            else
            {

                return BadRequest("Patient id must be defined.");
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
            await _context.Entry(slot).Reference(s => s.Disease)
               .LoadAsync();

            var mapped = _mapper.Map<Slot_GridDTO>(slot);

            return Ok(mapped);
        }

 

        // DELETE: api/Slots/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteSlot(int id)
        //{
        //    if (_context.Slots == null)
        //    {
        //        return NotFound();
        //    }
        //    var slot = await _context.Slots.FindAsync(id);
        //    if (slot == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Slots.Remove(slot);

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        return BadRequest($"{ex.Message}: {ex?.InnerException?.Message}");
        //    }

        //    return NoContent();
        //}

    }
}
