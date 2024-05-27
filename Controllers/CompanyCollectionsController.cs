using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Routine.Api.Helpers;
using Routine.Api.Models;
using Routine.Api.Services;

namespace Routine.Api.Controllers
{
    [ApiController]
    [Route("api/companycollections")]
    public class CompanyCollectionsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICompanyRepository _companyRepository;

        public CompanyCollectionsController(IMapper mapper, ICompanyRepository companyRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _companyRepository = companyRepository ?? throw new ArgumentException(nameof(companyRepository));
        }

        [HttpGet("({ids})", Name = nameof(GetCompanyCollection))]
        public async Task<IActionResult> GetCompanyCollection(
            [FromRoute] [ModelBinder(BinderType = typeof(ArrayModelBinder))]
            IEnumerable<Guid> ids)
        {
            // return 400 Bad Request if the ids are null
            if (ids == null)
            {
                return BadRequest();
            }

            var entities = await _companyRepository.GetCompaniesAsync(ids);

            // ids should be equal to entities
            if (ids.Count() != entities.Count())
            {
                return NotFound();
            }

            // transform the entities to DTOs
            var dtosToReturn = _mapper.Map<IEnumerable<CompanyDto>>(entities);

            return Ok(dtosToReturn);
        }


        [HttpPost]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> CreateCompanyCollection(
            IEnumerable<CompanyAddDto> companyCollection)
        {
            var companyEntities = _mapper.Map<IEnumerable<Entities.Company>>(companyCollection);

            foreach (var company in companyEntities)
            {
                _companyRepository.AddCompany(company);
            }

            await _companyRepository.SaveAsync();

            var dtosToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);

            var idsString = string.Join(",", dtosToReturn.Select(x => x.Id));

            // POST method: return 201 Created status code
            return CreatedAtRoute(nameof(GetCompanyCollection), new { ids = idsString }, dtosToReturn);
        }
    }
}