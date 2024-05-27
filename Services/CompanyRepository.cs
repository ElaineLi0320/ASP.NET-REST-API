using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Routine.Api.Data;
using Routine.Api.DtoParameters;
using Routine.Api.Entities;

namespace Routine.Api.Services
{
    /// <summary>
    /// Provides data access methods for company-related operations.
    /// </summary>
    public class CompanyRepository: ICompanyRepository
    {
        private readonly RoutineDbContext _context;
        
        /// <summary>
        /// Constructor: Initializes a new instance of the <see cref="CompanyRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
        public CompanyRepository(RoutineDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        /// <summary>
        /// Retrieves a list of companies based on the provided parameters.
        /// </summary>
        /// <param name="parameters">The parameters to filter and search the companies.</param>
        /// <returns>A list of companies that match the filter and search criteria.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the parameters argument is null.</exception>
        public async Task<IEnumerable<Company>> GetCompaniesAsync(CompanyDtoParameters parameters)
        { 
            // check if parameters is null
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            // check if company name and search term are not null or empty
            if (string.IsNullOrWhiteSpace(parameters.CompanyName) && string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                return await _context.Companies.ToListAsync();
            }
            
            var queryExpression = _context.Companies as IQueryable<Company>;
            
            // filtering
            if (!string.IsNullOrWhiteSpace(parameters.CompanyName))
            {
                parameters.CompanyName = parameters.CompanyName.Trim();
                queryExpression = queryExpression.Where(x => x.Name == parameters.CompanyName);
            }
            
            // searching
            if(!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                parameters.SearchTerm = parameters.SearchTerm.Trim();
                queryExpression = queryExpression.Where(x => x.Name.Contains(parameters.SearchTerm)
                                                             || x.Introduction.Contains(parameters.SearchTerm));
            }

            // pagination
            queryExpression = queryExpression.Skip(parameters.PageSize * (parameters.PageNumber - 1))
                .Take(parameters.PageSize);
            
            return await queryExpression.ToListAsync();
        }
        
        /// <summary>
        /// Retrieves a single company by its identifier.
        /// </summary>
        /// <param name="companyId">The identifier of the company.</param>
        /// <returns>The company if found; otherwise, null.</returns>
        /// <exception cref="ArgumentNullException">Thrown when companyId is empty.</exception>
        public async Task<Company> GetCompanyAsync(Guid companyId)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            return await _context.Companies.FirstOrDefaultAsync(x => x.Id == companyId);
        }
        
        /// <summary>
        /// Retrieves multiple companies by their identifiers.
        /// </summary>
        /// <param name="companyIds">The identifiers of the companies.</param>
        /// <returns>A list of companies.</returns>
        /// <exception cref="ArgumentNullException">Thrown when companyIds is null.</exception>
        public async Task<IEnumerable<Company>> GetCompaniesAsync(IEnumerable<Guid> companyIds)
        {
            if (companyIds == null)
            {
                throw new ArgumentNullException(nameof(companyIds));
            }

            return await _context.Companies
                .Where(x => companyIds.Contains(x.Id))
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
        
        /// <summary>
        /// Adds a new company.
        /// </summary>
        /// <param name="company">The company to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when company is null.</exception>
        public void AddCompany(Company company)
        {
            if (company == null)
            {
                throw new ArgumentNullException(nameof(company));
            }

            company.Id = Guid.NewGuid();

            if (company.Employees != null)
            {
                // add employees
                foreach (var employee in company.Employees)
                {
                    employee.Id = Guid.NewGuid();
                }
            }
            
            _context.Companies.Add(company);
        }
        
        /// <summary>
        /// Updates an existing company.
        /// </summary>
        /// <param name="company">The company to update.</param>
        public void UpdateCompany(Company company)
        {
            _context.Entry(company).State = EntityState.Modified;
        }

        /// <summary>
        /// Deletes a company.
        /// </summary>
        /// <param name="company">The company to delete.</param>
        /// <exception cref="ArgumentNullException">Thrown when company is null.</exception>
        public void DeleteCompany(Company company)
        {
            if (company == null)
            {
                throw new ArgumentNullException(nameof(company));
            }
            _context.Companies.Remove(company);
        }
        
        /// <summary>
        /// Checks if a company exists by its identifier.
        /// </summary>
        /// <param name="companyId">The identifier of the company.</param>
        /// <returns>True if the company exists; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when companyId is empty.</exception>
        public async Task<bool> CompanyExistsAsync(Guid companyId)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            return await _context.Companies.AnyAsync(x => x.Id == companyId);
        }
        
        /// <summary>
        /// Retrieves employees by company identifier with optional gender and search query filters.
        /// </summary>
        /// <param name="companyId">The identifier of the company.</param>
        /// <param name="genderDisplay">The gender to filter by.</param>
        /// <param name="q">The search query to filter by.</param>
        /// <returns>A list of employees.</returns>
        /// <exception cref="ArgumentNullException">Thrown when companyId is empty.</exception>
        public async Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, string genderDisplay, string q)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            
            // filtering and searching
            if (string.IsNullOrWhiteSpace(genderDisplay) && string.IsNullOrWhiteSpace(q))
            {
                return await _context.Employees
                    .Where(x => x.CompanyId == companyId)
                    .OrderBy(x => x.EmployeeNo)
                    .ToListAsync();
            }

            var items = _context.Employees.Where(x => x.CompanyId == companyId);

            if (!string.IsNullOrWhiteSpace(genderDisplay))
            {
                genderDisplay = genderDisplay.Trim();
                var gender = Enum.Parse<Gender>(genderDisplay);

                items = items.Where(x => x.Gender == gender);
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                
                items = items.Where(x => x.EmployeeNo.Contains(q)
                                                               || x.FirstName.Contains(q)
                                                               || x.LastName.Contains(q));
            }
            
            return await items.OrderBy(x => x.EmployeeNo).ToListAsync();
        }
        
        /// <summary>
        /// Retrieves an employee by company and employee identifiers.
        /// </summary>
        /// <param name="companyId">The identifier of the company.</param>
        /// <param name="employeeId">The identifier of the employee.</param>
        /// <returns>The employee if found; otherwise, null.</returns>
        /// <exception cref="ArgumentNullException">Thrown when companyId or employeeId is empty.</exception>
        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid employeeId)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            
            if (employeeId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(employeeId));
            }

            return await _context.Employees
                .Where(x => x.CompanyId == companyId && x.Id == employeeId)
                .FirstOrDefaultAsync();
        }
        
        /// <summary>
        /// Adds a new employee to a company.
        /// </summary>
        /// <param name="companyId">The identifier of the company.</param>
        /// <param name="employee">The employee to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when companyId or employee is empty.</exception>
        public void AddEmployee(Guid companyId, Employee employee)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            employee.CompanyId = companyId;
            _context.Employees.Add(employee);
        }
        
        /// <summary>
        /// Updates an existing employee.
        /// </summary>
        /// <param name="employee">The employee to update.</param>
        public void UpdateEmployee(Employee employee)
        {
            _context.Entry(employee).State = EntityState.Modified;
        }
        
        /// <summary>
        /// Deletes an employee.
        /// </summary>
        /// <param name="employee">The employee to delete.</param>
        public void DeleteEmployee(Employee employee)
        {
            _context.Employees.Remove(employee);
        }
        
        /// <summary>
        /// Saves changes to the database.
        /// </summary>
        /// <returns>True if save was successful; otherwise, false.</returns>
        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }
    }
}