using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Routine.Api.DtoParameters;
using Routine.Api.Entities;
using Routine.Api.Models;
using Routine.Api.Services;

namespace Routine.Api.Controllers
{
    /// <summary>
    /// Controller for handling company-related requests.
    /// </summary>
    [ApiController]
    [Route("api/companies")]
    public class CompaniesController: ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CompaniesController"/> class.
        /// </summary>
        /// <param name="companyRepository">The repository for company data.</param>
        /// <param name="mapper">The AutoMapper instance for mapping models.</param>
        /// <exception cref="ArgumentNullException">Thrown when the repository or mapper is null.</exception>
        public CompaniesController(ICompanyRepository companyRepository, IMapper mapper)
        {
            // Ensure the company repository is not null, otherwise throw an exception.
            _companyRepository = companyRepository ??
                                 throw new ArgumentNullException(nameof(companyRepository));
            // Ensure the AutoMapper instance is not null, otherwise throw an exception.
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        /// <summary>
        /// Gets the list of all companies.
        /// </summary>
        /// <returns>A list of company DTOs.</returns>
        [HttpGet]
        [HttpHead]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetCompanies([FromQuery]CompanyDtoParameters parameters)
        {
            var companies = await _companyRepository.GetCompaniesAsync(parameters);

            var companyDtos = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            
            return Ok(companyDtos);
        }
        
        /// <summary>
        /// Gets a single company by its identifier.
        /// </summary>
        /// <param name="companyId">The unique identifier of the company.</param>
        /// <returns>The company DTO if found; otherwise, a 404 Not Found response.</returns>
        [HttpGet("{companyId}", Name = nameof(GetCompany))]
        public async Task<ActionResult<CompanyDto>> GetCompany(Guid companyId)
        {
            var company = await _companyRepository.GetCompanyAsync(companyId);
            // Return 404 not found if we can not find the company
            if (company == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CompanyDto>(company));
        }
        
        /// <summary>
        /// Creates a new company.
        /// </summary>
        /// <param name="company">The data transfer object containing the details of the company to be created.</param>
        /// <returns>
        /// An ActionResult of type CompanyDto containing the created company details.
        /// Returns a CreatedAtRoute result with the route to the newly created company.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult<CompanyDto>> CreateCompany(CompanyAddDto company)
        {
            var entity = _mapper.Map<Company>(company);
            _companyRepository.AddCompany(entity);
            await _companyRepository.SaveAsync();
            
            var returnDto = _mapper.Map<CompanyDto>(entity);
            return CreatedAtRoute(nameof(GetCompany), new { companyId = returnDto.Id }, returnDto);
        }
        
        /// <summary>
        /// Deletes a company.
        /// </summary>
        /// <param name="companyId">The unique identifier of the company to be deleted.</param>
        /// <returns>
        /// An IActionResult indicating the result of the deletion operation.
        /// Returns NotFound if the company does not exist.
        /// Returns NoContent if the deletion is successful.
        /// </returns>
        [HttpDelete("{companyId}")]
        public async Task<IActionResult> DeleteCompany(Guid companyId)
        {
            var companyEntity = await _companyRepository.GetCompanyAsync(companyId);
            
            // check if the company exists
            if (companyEntity == null)
            {
                return NotFound();
            }

            await _companyRepository.GetEmployeesAsync(companyId, null, null);
            
            _companyRepository.DeleteCompany(companyEntity);
            
            await _companyRepository.SaveAsync();
            
            return NoContent();
        }
        
        /// <summary>
        /// Returns the HTTP methods that are allowed for the companies endpoint.
        /// </summary>
        /// <returns>
        /// An IActionResult with the allowed HTTP methods in the response headers.
        /// Returns an Ok result.
        /// </returns>
        [HttpOptions]
        public IActionResult GetCompaniesOptions()
        {
            Response.Headers.Add("Allow", "GET, POST, OPTIONS");
            return Ok();
        }
        
    }
}