using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalAdmissionApp.Server.Model;

namespace HospitalAdmissionApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SlotsController : ControllerBase
    {
        private readonly DataContext _context;

        public SlotsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Slots
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Slot>>> GetSlots()
        {
          if (_context.Slots == null)
          {
              return NotFound();
          }
            return await _context.Slots.ToListAsync();
        }

        // GET: api/Slots/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Slot>> GetSlot(int id)
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

            return slot;
        }

        // PUT: api/Slots/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSlot(int id, Slot slot)
        {
            if (id != slot.Id)
            {
                return BadRequest();
            }

            _context.Entry(slot).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SlotExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Slots
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Slot>> PostSlot(Slot slot)
        {
          if (_context.Slots == null)
          {
              return Problem("Entity set 'DataContext.Slots'  is null.");
          }
            _context.Slots.Add(slot);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSlot", new { id = slot.Id }, slot);
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
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SlotExists(int id)
        {
            return (_context.Slots?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
