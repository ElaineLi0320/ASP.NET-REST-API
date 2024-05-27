using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Routine.Api.Entities;
using Routine.Api.Models;
using Routine.Api.Services;

namespace Routine.Api.Controllers
{
    /// <summary>
    /// Controller for handling employee-related requests for a specific company.
    /// </summary>
    [ApiController]
    [Route("api/companies/{companyId}/employees")]
    public class EmployeesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICompanyRepository _companyRepository;

        /// <summary>
        /// Constructor: Initializes a new instance of the <see cref="EmployeesController"/> class.
        /// </summary>
        /// <param name="mapper">The AutoMapper instance for mapping models.</param>
        /// <param name="companyRepository">The repository for company data.</param>
        /// <exception cref="ArgumentNullException">Thrown when the repository or mapper is null.</exception>
        public EmployeesController(IMapper mapper, ICompanyRepository companyRepository)
        {
            _mapper = mapper ?? throw new ArgumentException(nameof(mapper));
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
        }

        /// <summary>
        /// Gets all employees for a specific company.
        /// </summary>
        /// <param name="companyId">The unique identifier of the company.</param>
        /// <param name="genderDisplay">Optional gender filter for employees.</param>
        /// <param name="q">Optional query parameter for employee search.</param>
        /// <returns>A list of employee DTOs.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> 
            GetEmployeesForCompany(Guid companyId, [FromQuery]string genderDisplay, string q)
        {
            // check if the company exist
            if (!await _companyRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }

            // if company exists, return the employees of that company
            var employees = await _companyRepository.GetEmployeesAsync(companyId, genderDisplay, q);
            var employeeDtos = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
            return Ok(employeeDtos);
        }

        /// <summary>
        /// Gets a single employee from a specific company.
        /// </summary>
        /// <param name="companyId">The unique identifier of the company.</param>
        /// <param name="employeeId">The unique identifier of the employee.</param>
        /// <returns>The employee DTO if found; otherwise, a 404 Not Found response.</returns>
        [HttpGet("{employeeId}", Name = nameof(GetEmployeeForCompany))]
        public async Task<ActionResult<EmployeeDto>> GetEmployeeForCompany(Guid companyId, Guid employeeId)
        {
            // check if the company exist
            if (!await _companyRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }

            // check if the employee exist
            var employee = await _companyRepository.GetEmployeeAsync(companyId, employeeId);
            if (employee == null)
            {
                return NotFound();
            }

            // if company exists, return the selected employee of that company
            var employeeDto = _mapper.Map<EmployeeDto>(employee);
            return Ok(employeeDto);
        }
        
        /// <summary>
        /// Creates a new employee for a specified company.
        /// </summary>
        /// <param name="companyId">The ID of the company.</param>
        /// <param name="employee">The employee data to add.</param>
        /// <returns>An ActionResult of type EmployeeDto representing the created employee.</returns>
        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> CreateEmployeeForCompany(Guid companyId, EmployeeAddDto employee)
        {
            // check if the company with the given id exists
            if (!await _companyRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }

            var entity = _mapper.Map<Employee>(employee);
            
            _companyRepository.AddEmployee(companyId, entity);
            await _companyRepository.SaveAsync();
            
            // Map the employee entity back to a data transfer object to return
            var dtoToReturn = _mapper.Map<EmployeeDto>(entity);
            // Return a 201 Created response with the route to retrieve the newly created employee
            return CreatedAtRoute(nameof(GetEmployeeForCompany), new
            {
                companyId,
                employeeId = dtoToReturn.Id
            }, dtoToReturn);
        }
        
        /// <summary>
        /// Updates an existing employee for a specified company or adds a new employee if the employee does not exist.
        /// </summary>
        /// <param name="companyId">The ID of the company.</param>
        /// <param name="employeeId">The ID of the employee to be updated or added.</param>
        /// <param name="employee">The data transfer object containing updated employee information.</param>
        /// <returns>
        /// An ActionResult containing the updated EmployeeDto if a new employee is added,
        /// or a NoContent result if an existing employee is updated.
        /// </returns>
        /// <response code="201">Returns the newly created employee.</response>
        /// <response code="204">Indicates that the existing employee was updated successfully.</response>
        /// <response code="404">Indicates that the specified company does not exist.</response>
        [HttpPut("{employeeId}")]
        public async Task<ActionResult<EmployeeDto>> UpdateEmployeeForCompany(
            Guid companyId, Guid employeeId, EmployeeUpdateDto employee)
        {
            // Check if company exists
            if (!await _companyRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }
            
            // Check if employee exists
            var employeeEntity = await _companyRepository.GetEmployeeAsync(companyId, employeeId);
            if (employeeEntity == null)
            { // If the employee does not exist, add it
               var employeeToAddEntity = _mapper.Map<Employee>(employee);
               employeeToAddEntity.Id = employeeId;
               employeeToAddEntity.CompanyId = companyId;
               
               _companyRepository.AddEmployee(companyId, employeeToAddEntity);
               
               // save
               await _companyRepository.SaveAsync();
               
               var dtoToReturn = _mapper.Map<EmployeeDto>(employeeToAddEntity);
              
               return CreatedAtRoute(nameof(GetEmployeeForCompany), new
               {
                   companyId,
                   employeeId = dtoToReturn.Id
               }, dtoToReturn);

            }
            
            // Map the employee entity to the updated employee data transfer object
            _mapper.Map(employee, employeeEntity);
            _companyRepository.UpdateEmployee(employeeEntity);
            
            await _companyRepository.SaveAsync();
            return NoContent();
        }
        
        /// <summary>
        /// partially update an employee for a specific company
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="employeeId"></param>
        /// <param name="patchDocument"></param>
        /// <returns> Patched employee data transfer object if successful; otherwise, a 404 Not Found response.
        /// </returns>
        /// <Response code="204">Indicates that the employee was updated successfully.</Response>
        [HttpPatch("{employeeId}")]
        public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(
            Guid companyId, Guid employeeId, JsonPatchDocument<EmployeeUpdateDto> patchDocument)
        {
            // Check if company exists
            if (!await _companyRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }
            
            // Check if employee exists
            var employeeEntity = await _companyRepository.GetEmployeeAsync(companyId, employeeId);
            if (employeeEntity == null)
            {
                // add a new employee if the employee does not exist
                var employeeDto = new EmployeeUpdateDto();
                patchDocument.ApplyTo(employeeDto,ModelState);

                if (!TryValidateModel(employeeDto))
                {
                    return ValidationProblem(ModelState);
                }
                
                var employeeToAdd = _mapper.Map<Employee>(employeeDto);
                employeeToAdd.Id = employeeId;
                
                _companyRepository.AddEmployee(companyId, employeeToAdd);
                await _companyRepository.SaveAsync();
                
                var dtoToReturn = _mapper.Map<EmployeeDto>(employeeToAdd);
                
                return CreatedAtRoute(nameof(GetEmployeeForCompany), new
                {
                    companyId,
                    employeeId = dtoToReturn.Id
                }, dtoToReturn);
            }
            
            var dtoToPatch = _mapper.Map<EmployeeUpdateDto>(employeeEntity);
            
            patchDocument.ApplyTo(dtoToPatch);

            if (!TryValidateModel(dtoToPatch))
            {
                return ValidationProblem(ModelState);
            }
            
            _mapper.Map(dtoToPatch, employeeEntity);
            
            _companyRepository.UpdateEmployee(employeeEntity);
            
            await _companyRepository.SaveAsync();

            return NoContent();
        }
        
        /// <summary>
        /// Deletes an employee for a specific company.
        /// </summary>
        /// <param name="companyId">The unique identifier of the company.</param>
        /// <param name="employeeId">The unique identifier of the employee to be deleted.</param>
        /// <returns>An IActionResult indicating the result of the deletion operation.</returns>
        /// <remarks>
        /// If the company or employee does not exist, a NotFound result is returned.
        /// If the deletion is successful, a NoContent result is returned.
        /// </remarks>
        [HttpDelete("{employeeId}")]
        public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId, Guid employeeId)
        {
            // Check if company exists
            if (!await _companyRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }
            
            // Check if employee exists
            var employeeEntity = await _companyRepository.GetEmployeeAsync(companyId, employeeId);
            if (employeeEntity == null)
            {
                return NotFound();
            }
            
            _companyRepository.DeleteEmployee(employeeEntity);
            
            await _companyRepository.SaveAsync();

            return NoContent();
        }
        
        /// <summary>
        /// Validates the model state and returns a validation problem response.
        /// </summary>
        /// <param name="modelStateDictionary"></param>
        /// <returns> A validation problem response containing the model state dictionary.
        /// </returns>
        public override ActionResult ValidationProblem(ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
    }
}