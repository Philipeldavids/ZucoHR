using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZucoHR.Application.Interfaces;

namespace ZucoHR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;    
        }
        [AllowAnonymous]
        [HttpGet("{companySlug}/public")]
        public async Task<IActionResult> GetPublicProfile(string  companySlug)
        {
            var result =
                await _companyService
                    .GetPublicCompanyProfileAsync(companySlug);

            return Ok(result);
        }
    }
}
