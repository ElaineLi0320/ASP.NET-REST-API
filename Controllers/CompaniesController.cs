using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Routine.Api.Services;

namespace Routine.Api.Controllers
{
    [ApiController]
    [Route("api/companies")]
    public class CompaniesController: ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
            
        public CompaniesController(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository ??
                                 throw new ArgumentNullException(nameof(companyRepository));
        }
        
        // Get companies
        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = _companyRepository.GetCompaniesAsync();
            
            return Ok(companies);
        }
        
        // Get a single company
        [HttpGet("{companyId}")]
        public async Task<IActionResult> GetCompany(Guid companyId)
        {
            var company = await _companyRepository.GetCompanyAsync(companyId);
            // Return 404 not found if we can not find the company
            if (company == null)
            {
                return NotFound();
            }
            return Ok(company);
        }
        
        
        
        
        
    }
}