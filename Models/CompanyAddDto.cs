using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Routine.Api.Models
{
    /// <summary>
    /// Data Transfer Object for adding a new company.
    /// </summary>
    public class CompanyAddDto
    {
        /// <summary>
        /// Gets or sets the name of the company.
        /// </summary>
        /// <value>The name of the company.</value>
        [Display(Name = "Company Name")]
        [Required(ErrorMessage = "The {0} field is required.")]
        [MaxLength(100, ErrorMessage = "The maximum length for {0} cannot exceed {1} characters.")]
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the description of the company.
        /// </summary>
        /// <value>The description of the company.</value>
        [Display(Name = "Description")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "The length of {0} must be between {2} and {1} characters.")]
        public string Introduction { get; set; }
        
        /// <summary>
        /// Gets or sets the collection of employees to be added to the company.
        /// </summary>
        /// <value>A collection of EmployeeAddDto objects.</value>
        public ICollection<EmployeeAddDto> Employees { get; set; } = new List<EmployeeAddDto>();
    }
}