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

            var entity = _mapper.Map<Bed>(dto);

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

            return CreatedAtAction("GetBed", new { id = mapped.Id }, mapped);
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
