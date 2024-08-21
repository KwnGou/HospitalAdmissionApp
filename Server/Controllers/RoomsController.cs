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
using Microsoft.Data.SqlClient;
using System.Linq.Dynamic.Core;
using HospitalAdmissionApp.Shared;

namespace HospitalAdmissionApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public RoomsController(DataContext context, IMapper mapper, IConfiguration config)
        {
            _context = context;
            _mapper = mapper;
            _config = config;
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

        // GET: api/Rooms/5/roomInfo
        [HttpGet("roomInfo", Name = nameof(GetRoomInfo))]
        public async Task<ActionResult<string>> GetRoomInfo(
            [FromQuery(Name = "b")] int? bedId)
        {
            if (_context.Beds == null)
            {
                return NotFound();
            }

            if (_context.Rooms == null)
            {
                return NotFound();
            }

            if (bedId == null)
            {
                return BadRequest("Bed id is required.");
            }

            var selectedBed = await _context.Beds.FindAsync(bedId);

            if (selectedBed == null)
            {
                return NotFound();
            }

            List<object> parameters = new() {
                new SqlParameter("SelectedBedId", selectedBed.Id)};

            var cmdTxt = @"
SELECT B.BedInfo AS BedInfo, P.DateOfBirth AS PatientDateOfBirth, P.Sex AS PatientSex, D.[Name] AS PatientDiseaseName
FROM Slots S
JOIN Patients P ON S.PatientId = P.Id
JOIN Beds B ON S.BedId = B.Id
JOIN Diseases D ON S.DiseaseId = D.Id
WHERE S.ReleaseDate IS NULL AND B.RoomId = (SELECT RoomId FROM Beds WHERE Id = @SelectedBedId)
";


            try
            {
                var result = await _context.RoomUsedBedsInfo.FromSqlRaw(cmdTxt, parameters.ToArray()).ToListAsync();

                var head = "This room has:\n\n";
                if (result.Count > 0)
                {
                    var resStr = result.Aggregate("",
                        (current, next) => current + $"Bed: {next.BedInfo}, Disease: {next.PatientDiseaseName}, Sex: {SexName(next.PatientSex)}, Age: {PatientAge(next.PatientDateOfBirth)}\n");

                    return Ok(head + resStr);
                }
                else
                {
                    return Ok("No other patients in the room");
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}: {ex?.InnerException?.Message}");
            }
        }

        #region Helpers
        
        private string SexName(int sexId)
        {
            var sexOptions = _config.GetSection(EnumOption.SexOptionsString).Get<EnumOption[]>();
            return sexOptions.First(s => s.Id == sexId).Text;
        }

        private static int PatientAge(DateTime birthDate)
        {
            return DateTime.Now.Year - birthDate.Year;            
        }

        #endregion

        // GET : api/Rooms/roomDetails
        [HttpGet("roomDetails", Name = nameof(GetRoomDetails))]
        public async Task<ActionResult<IEnumerable<RoomOverview_DTO>>>GetRoomDetails(
            [FromQuery(Name = "c")] int? clinicId)
        {
            if (_context.Clinics == null)
            {
                return NotFound();
            }
            if (_context.Rooms == null)
            {
                return NotFound();
            }
            if (_context.Beds == null)
            {
                return NotFound();
            }

            if (clinicId == null) 
            {
                return BadRequest();
            }

            var selectedClinic = await _context.Clinics.FindAsync(clinicId);

            if (selectedClinic == null)
            {
                return NotFound();
            }

            List<object> parameters = new() {
                new SqlParameter("SelectedClinicId", selectedClinic.Id)};


            var cmdTxt = @"
SELECT R.Id AS RoomId, R.RoomNumber AS RoomNumber
	, B.Id AS BedId, B.BedInfo AS BedInfo
	, P.Id AS PatientId, P.[Name] AS PatientName, P.Surname AS PatientSurname
	, D.[Name] AS DiseaseName
FROM Rooms R
JOIN Beds B ON R.Id = B.RoomId
LEFT OUTER JOIN (SELECT S1.Id, S1.BedId, S1.PatientId, S1.DiseaseId FROM SLOTS S1 WHERE S1.ReleaseDate IS NULL) S2 ON S2.BedId = B.Id
LEFT OUTER JOIN Patients P ON S2.PatientId = P.Id
LEFT OUTER JOIN Diseases D ON S2.DiseaseId = D.Id
WHERE R.ClinicId = @SelectedClinicId
ORDER BY R.Id, B.Id
";
/* , CASE WHEN EXISTS 
	(
		SELECT S.Id FROM Slots S WHERE S.ReleaseDate IS NULL AND B.Id = S.BedId
	)
    THEN CAST (1 AS BIT)
    ELSE CAST (0 AS BIT)
    END AS Occupied
*/
            try
            {
                var result = await _context.RoomDetailsList.FromSqlRaw(cmdTxt, parameters.ToArray()).ToListAsync();
                var mapped = result.Select(r => new RoomOverview_DTO { RoomId = r.RoomId, RoomNumber = r.RoomNumber, Beds = new List<BedOverview_DTO>() }).DistinctBy(r => r.RoomId).ToList();
                foreach (var item in mapped)
                {
                    item.Beds = result.Where(r => r.RoomId == item.RoomId).Select(r => new BedOverview_DTO {
                        BedId = r.BedId, BedInfo = r.BedInfo, DiseaseName = r.DiseaseName,
                        PatientId = r.PatientId, PatientFullName = $" {(string.IsNullOrWhiteSpace(r.PatientName) ? string.Empty : r.PatientName)} {(string.IsNullOrWhiteSpace(r.PatientSurname) ? string.Empty : r.PatientSurname.ToUpper())} ".Trim() })
                        .ToList();
                    item.Occupied = item.Beds.Any(i => i.PatientId.HasValue);
                }
                return Ok(mapped);

            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}: {ex?.InnerException?.Message}");
            }
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
