using System.ComponentModel.DataAnnotations;
using Routine.Api.Models;

namespace Routine.Api.ValidationAttributes
{
    /// <summary>
    /// Custom validation attribute to ensure that the employee number is different from the first name.
    /// </summary>
    public class EmployeeNoMustDifferentFromFirstNameAttribute : ValidationAttribute
    {
        /// <summary>
        /// Validates that the employee number is different from the first name.
        /// </summary>
        /// <param name="value">The value of the object to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>A <see cref="ValidationResult"/> indicating whether validation succeeded or failed.</returns>

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var addDto = (EmployeeAddOrUpdateDto)validationContext.ObjectInstance;

            // Check if EmployeeNo is equal to FirstName
            if (addDto.EmployeeNo == addDto.FirstName)
            {
                return new ValidationResult(ErrorMessage, new[] { nameof(EmployeeAddOrUpdateDto) });
            }

            return ValidationResult.Success;
        }
    }
}