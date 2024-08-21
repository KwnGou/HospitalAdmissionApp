using AutoMapper;
using HospitalAdmissionApp.Server.Model;
using HospitalAdmissionApp.Shared;
using HospitalAdmissionApp.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace HospitalAdmissionApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly string _patientIdCardRx;
        private readonly string _patientIdCardRxError;

        public PatientsController(DataContext context, IMapper mapper, IConfiguration config)
        {
            _context = context;
            _mapper = mapper;
            _config = config;

            var appOptions = _config.GetSection(AppConfigOptions.AppConfigOptionsSection).Get<AppConfigOptions>();
            _patientIdCardRx = appOptions.PatientIdCardRx;
            _patientIdCardRxError = appOptions.PatientIdCardRxError;
        }

        // GET: api/Patients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient_GridDTO>>> GetPatients()
        {
            if (_context.Patients == null)
            {
                return NotFound();
            }
            //List<Patient> result;
            //, P2.[Name] AS [Name], P2.Surname AS Surname, P2.PatientIdentityCard AS PatientIdentityCard not needed? inner select
            var cmdTxt = @"
SELECT P1.Id AS Id, P1.[Name] AS [Name], P1.Surname AS Surname, P1.PatientIdentityCard AS PatientIdentityCard, CASE WHEN EXISTS 
	(SELECT P2.Id AS Id
	FROM Patients P2
	JOIN Slots S1 ON P2.Id = S1.PatientId
	WHERE P1.Id = P2.ID AND S1.ReleaseDate IS NULL
	)
THEN CAST(1 AS BIT)
ELSE CAST(0 AS BIT) 
END AS Hospitalized
FROM PATIENTS P1
";
            try
            {
                var result = await _context.PatientsList.FromSqlRaw(cmdTxt).ToListAsync();
                return Ok(result);
            }
            catch (Exception ex) 
            {
                return BadRequest($"{ex.Message}: {ex?.InnerException?.Message}");
            }
            //result = await _context.Patients
            //    .ToListAsync();
            //var mapped = _mapper.Map<IEnumerable<Patient_GridDTO>>(result);

            //return Ok(mapped);
        }
        // GET: api/Patients/patientsForAdmission
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
WHERE P1.Id NOT IN (
	SELECT Slots.PatientId FROM Slots
	)
  UNION
SELECT P2.Id AS Id, P2.[Name] AS [Name], P2.Surname AS Surname, P2.PatientIdentityCard AS PatientIdentityCard
FROM Patients P2
WHERE NOT P2.Id IN (
	SELECT P3.Id AS Id
	FROM Patients P3
	JOIN Slots S2
	ON P3.Id = S2.PatientId
	WHERE S2.ReleaseDate IS NULL
	)
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

            var sexOptions = _config.GetSection(EnumOption.SexOptionsString).Get<EnumOption[]>();
            var insuranceOptions = _config.GetSection(EnumOption.InsuranceOptionsString).Get<EnumOption[]>();

            mapped.SexText = sexOptions.First(s => s.Id == mapped.Sex).Text;
            mapped.InsuranceText = insuranceOptions.First(i => i.Id == mapped.Insurance).Text;
            mapped.Age = DateTime.Now.Year - mapped.DateOfBirth.Year;

            mapped.History = new List<PatientHistory_GridDTO>();

            var patientHistory = await _context.Slots
                .Include(s => s.Disease)
                .Include(s => s.Bed)
                .ThenInclude(b => b.Room)
                .Where(s => s.PatientId == id).OrderBy(s => s.AdmissionDate).ToArrayAsync();

            var mapHistory = _mapper.Map<PatientHistory_GridDTO[]>(patientHistory);

            mapped.History.AddRange(mapHistory);

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
            dto.Name = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(dto.Name.ToLower());

            if (string.IsNullOrWhiteSpace(dto.Surname))
            {
                return Problem("Patient surname is required.");
            }

            dto.Surname.Trim();
            dto.Surname = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(dto.Surname.ToLower());

            if (string.IsNullOrWhiteSpace(dto.PatientIdentityCard))
            {
                return Problem("Patient PatientIdentityCard is required.");
            }

            dto.PatientIdentityCard.Trim();

            if (!Regex.IsMatch(dto.PatientIdentityCard, _patientIdCardRx))
            {
                return BadRequest(_patientIdCardRxError);
            }
            var entity = _mapper.Map<Patient>(dto);

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
            dto.Name = char.ToUpper(dto.Name[0]) + dto.Name.Substring(1);

            if (string.IsNullOrWhiteSpace(dto.Surname))
            {
                return Problem("Patient surname is required.");
            }

            dto.Surname.Trim();
            dto.Surname = char.ToUpper(dto.Surname[0]) + dto.Surname.Substring(1);

            if (string.IsNullOrWhiteSpace(dto.PatientIdentityCard))
            {
                return Problem("Patient PatientIdentityCard is required.");
            }

            dto.PatientIdentityCard.Trim();

            if (!Regex.IsMatch(dto.PatientIdentityCard, _patientIdCardRx))
            {
                return BadRequest(_patientIdCardRxError);
            }
            
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

            if (_context.Slots.Any(s => s.PatientId == id && s.ReleaseDate == null))
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

        private bool SlotExists(int id)
        {
            return (_context.Slots?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // PUT: api/Patients/DismissPatient
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("DismissPatient", Name = nameof(DismissPatient))]
        public async Task<IActionResult> DismissPatient(SlotSelection_DTO dto)
        {
            if (_context.Slots == null)
            {
                return Problem("Entity set 'DataContext.Slots'  is null.");
            }

            //Data Validation (ID existence)
            var (res, msg) = await ValidateDataSlot(dto);
            if (!res)
            {
                return BadRequest(msg);
            }

            var entity = await _context.Slots.FindAsync(dto.Id);
            if (entity == null)
            {
                return BadRequest("Specified slot does not exist.");
            }

            // check combination 
            if (entity.PatientId != dto.PatientId || entity.BedId != dto.BedId || entity.DiseaseId != dto.DiseaseId)
            {
                return BadRequest("Invalid data");
            }

            // check if patient has been dismissed already
            if (entity.ReleaseDate != null)
            {

                return BadRequest("Specified patient has been dismissed.");
            }

            _context.Entry(entity).State = EntityState.Modified;
            entity.ReleaseDate = DateTimeOffset.Now;


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

        // POST: api/Patients/AdmitPatient
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("AdmitPatient", Name = nameof(AdmitPatient))]
        public async Task<ActionResult<Slot>> AdmitPatient(SlotSelection_DTO dto)
        {
            if (_context.Slots == null)
            {
                return Problem("Entity set 'DataContext.Slots'  is null.");
            }
            var entity = _mapper.Map<Slot>(dto);

            //Data Validation
            var (res, msg) = await ValidateDataSlot(dto);
            if (!res)
            {
                return BadRequest(msg);
            }
            // check if patient has been hospitalized already
            if (await _context.Slots.AnyAsync(s => s.ReleaseDate == null && s.PatientId == dto.PatientId))
            {

                return BadRequest("Specified patient is already hospitalised.");
            }
            entity.AdmissionDate = DateTimeOffset.Now;

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

            return CreatedAtAction("GetPatient", new { id = mapped.PatientId }, mapped);
        }

        private async Task<(bool result, string message)> ValidateDataSlot(SlotSelection_DTO dto)
        {

            if (!(await _context.Patients.AnyAsync(p => p.Id == dto.PatientId)))
            {
                return (false, "Specified patient id does not exist.");
            }

            if (!(await _context.Beds.AnyAsync(b => b.Id == dto.BedId)))
            {
                return (false, "Specified bed id does not exist.");
            }

            if (!(await _context.Diseases.AnyAsync(d => d.Id == dto.DiseaseId)))
            {
                return (false, "Specified disease id does not exist.");
            }

            return (true, string.Empty);
        }
    }
}
