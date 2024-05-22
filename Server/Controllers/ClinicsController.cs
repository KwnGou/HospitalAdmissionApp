using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using HospitalAdmissionApp.Server.Model;
using HospitalAdmissionApp.Shared.DTOs;


namespace HospitalAdmissionApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public ClinicsController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Clinics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Clinic_GridDTO>>> GetClinics()
        {
            if (_context.Clinics == null)
            {
                return NotFound();
            }

            var result = await _context.Clinics.ToListAsync();
            var mapped = _mapper.Map<IEnumerable<Clinic_GridDTO>>(result);

            return Ok(mapped);
        }

        // GET: api/Clinics/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Clinic_GridDTO>> GetClinic(int id)
        {
            if (_context.Clinics == null)
            {
                return NotFound();
            }

            var result = await _context.Clinics.FindAsync(id);

            if (result == null)
            {
                return NotFound();
            }
            var mapped = _mapper.Map<Clinic_GridDTO>(result);

            return Ok(mapped);
        }

        // PUT: api/Clinics/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClinic(int id, Clinic_GridDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            //Data Validation
            if (!(await _context.Clinics.AllAsync(c => c.Id == dto.Id)))
            {
                return BadRequest("Specified clinic id does not exist.");
            }

            var (res, msg) = await ValidateData(dto);
            if (!res)
            {
                return BadRequest(msg);
            }

            var entity = _mapper.Map<Clinic_GridDTO>(dto);

            _context.Entry(entity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            //the application is not going to support multiple users the following exception is not needed at the moment
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!ClinicExists(id))
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

        // POST: api/Clinics
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Clinic_GridDTO>> PostClinic(Clinic_GridDTO dto)
        {
            if (_context.Clinics == null)
            {
                return Problem("Entity set 'HospitalAdmissionDBContext.Clinic' is null.");
            }

            //Data validation
            var (res, msg) = await ValidateData(dto);
            if (!res)
            {
                return BadRequest(msg);
            }

            var entity = _mapper.Map<Clinic>(dto);

            _context.Clinics.Add(entity);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest($"{ex.Message} : {ex?.InnerException?.Message}");
            }

            var mapped = _mapper.Map<Clinic_GridDTO>(entity);

            return CreatedAtAction("GetClinic", new { id = mapped.Id }, mapped);
        }

        // DELETE: api/Clinics/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClinic(int id)
        {
            if (_context.Clinics == null)
            {
                return Problem("Entity set 'HospitalAdmissionDBContext.Clinic' is null.");
            }

            var clinic = await _context.Clinics.FindAsync(id);
            if (clinic == null)
            {
                return NotFound();
            }

            if (await _context.Diseases.AnyAsync(d => d.ClinicId == id))
            {
                return BadRequest("Clinic is in use.");
            }

            _context.Clinics.Remove(clinic);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest($"{ex.Message} : {ex?.InnerException?.Message}");
            }

            return NoContent();
        }

        private bool ClinicExists(int id)
        {
            return _context.Clinics.Any(e => e.Id == id);
        }

        private async Task<(bool result, string message)> ValidateData(Clinic_GridDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return (false, "Clinic name is required.");
            }

            dto.Name.Trim();

            if (await _context.Clinics.AnyAsync(c => c.Name == dto.Name))
            {
                return (false, "Specified clinic name already exists.");
            }

            return (true, string.Empty);
        }
    }
}
