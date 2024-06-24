using HospitalAdmissionApp.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAdmissionApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnumOptionsController : ControllerBase
    {
        private readonly IConfiguration _config;
        public EnumOptionsController(IConfiguration config)
        {
            _config = config;
        }

        // GET: api/EnumOptions/sexOptions
        [HttpGet("sexOptions", Name = nameof(GetSexOptions))]
        public ActionResult<IEnumerable<EnumOption>> GetSexOptions() 
        {
            return Ok(_config.GetSection(EnumOption.SexOptionsString).Get<EnumOption[]>());
        }

        // GET: api/EnumOptions/insuranceOptions
        [HttpGet("insuranceOptions", Name = nameof(GetInsuranceOptions))]
        public ActionResult<IEnumerable<EnumOption>> GetInsuranceOptions()
        {
            return Ok(_config.GetSection(EnumOption.InsuranceOptionsString).Get<EnumOption[]>());
        }
    }
}
