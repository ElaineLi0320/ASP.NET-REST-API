using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Routine.Api.Entities;
using Routine.Api.ValidationAttributes;

namespace Routine.Api.Models
{
    [EmployeeNoMustDifferentFromFirstName(ErrorMessage = "Employee Number cannot be the same as First Name.")]
    
    public abstract class EmployeeAddOrUpdateDto: IValidatableObject
    {
        [Display(Name = "Employee Number")]
        [Required(ErrorMessage = "{0} is required.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "The length of {0} must be {1} characters.")]
        public string EmployeeNo { get; set; }
        
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "{0} is required.")]
        [MaxLength(50, ErrorMessage = "The length of {0} cannot exceed {1} characters.")]
        public string FirstName { get; set; }
        
        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "{0} is required.")]
        [MaxLength(50, ErrorMessage = "The length of {0} cannot exceed {1} characters.")]
        public string LastName { get; set; }
        
        [Display(Name = "Gender")]
        public Gender Gender { get; set; }
        
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FirstName == LastName)
            {
                yield return new ValidationResult("First Name and Last Name cannot be the same",
                    new[] { nameof(FirstName), nameof(LastName) });
            }
        }
    }
}