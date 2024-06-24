using HospitalAdmissionApp.Server.Model;
using HospitalAdmissionApp.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace HospitalAdmissionApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToolsController : Controller
    {
        private readonly IConfiguration _config;
        private readonly DataContext _context;
        private readonly AppConfigOptions _appOptions;


        public ToolsController(IConfiguration config, DataContext context)
        {
            _config = config;
            _context = context;

            _appOptions = _config.GetSection(AppConfigOptions.AppConfigOptionsSection).Get<AppConfigOptions>();
        }

        //GET: api/Tools/AppConfigOptions
        [HttpGet("appConfigOptions", Name = nameof(GetAppConfigOptions))]
        public ActionResult<AppConfigOptions> GetAppConfigOptions()
        {
            return Ok(_appOptions);
        }
    }
}
