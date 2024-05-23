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
    public class RoomsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public RoomsController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Rooms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room_GridDTO>>> GetRooms([FromQuery] int? clinicId)
        {
          if (_context.Rooms == null)
          {
              return NotFound();
          }
            List<Room> result;

            if (clinicId.HasValue)
            {
                result = await _context.Rooms
                    .Where(r => r.ClinicId == clinicId.Value)
                    .Include(c => c.Clinic)
                    .ToListAsync();
            }
            else
            {
                result = await _context.Rooms
                    .Include(r => r.Clinic)
                    .ToListAsync();
            }

            var mapped = _mapper.Map<IEnumerable<Room_GridDTO>>(result);

            return Ok(mapped);
        }

        // GET: api/Rooms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Room_GridDTO>> GetRoom(int id)
        {
          if (_context.Rooms == null)
          {
              return NotFound();
          }
            var room = await _context.Diseases.FindAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            await _context.Entry(room).Reference(r => r.Clinic).LoadAsync();

            var mapped = _mapper.Map<Room_GridDTO>(room);

            return Ok(mapped);
        }

        // PUT: api/Rooms/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoom(int id, Room_GridDTO dto)
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

            var entity = _mapper.Map<Room>(dto);

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

        // POST: api/Rooms
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Room>> PostRoom(Room_GridDTO dto)
        {
            if (_context.Rooms == null)
            {
                return Problem("Entity set 'DataContext.Rooms'  is null.");
            }

            var entity = _mapper.Map<Room>(dto);

            //Data Validation
            var (res, msg) = await ValidateData(dto);
            if (!res)
            {
                return BadRequest(msg);
            }

            _context.Rooms.Add(entity);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest($"{ex.Message}: {ex?.InnerException?.Message}");
            }

            await _context.Entry(entity).Reference(r => r.Clinic).LoadAsync();

            var mapped = _mapper.Map<Room_GridDTO>(entity);

            return CreatedAtAction("GetRooms", new { id = mapped.Id }, mapped);
        }

        // DELETE: api/Rooms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            if (_context.Rooms == null)
            {
                return NotFound();
            }
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            if (await _context.Beds.AllAsync(b => b.RoomId == id))
            {
                return BadRequest("There is a patient in the specific room.");
            }

            _context.Rooms.Remove(room);

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

        private bool RoomExists(int id)
        {
            return (_context.Rooms?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private async Task<(bool result, string message)> ValidateData(Room_GridDTO dto)
        {
            if (dto.RoomNumber <= 0 || dto.RoomNumber > 800)
            {
                return (false, "Room numbers start from 1 and reach 800.");
            }


            if (await _context.Rooms.AnyAsync(r => r.RoomNumber == dto.RoomNumber))
            {
                return (false, "Specified room already exist.");
            }

            if (!(await _context.Clinics.AnyAsync(c => c.Id == dto.ClinicId)))
            {
                return (false, "Specified clinic does not exist.");
            }

            return (true, string.Empty);
        }
    }
}
