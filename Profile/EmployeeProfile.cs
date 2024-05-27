using System;
using Routine.Api.Entities;
using Routine.Api.Models;

namespace Routine.Api.Profile
{
    // Constructor for EmployeeProfile class
    public class EmployeeProfile: AutoMapper.Profile
    {
        // Creating a mapping configuration from Employee entity to EmployeeDto model
        public EmployeeProfile()
        {
            CreateMap<Employee, EmployeeDto>()
                .ForMember(
                    dest => dest.Name,
                    opt => opt.MapFrom(src => $"{src.FirstName}{src.LastName}"))
                .ForMember(
                    dest => dest.GenderDisplay,
                    opt => opt.MapFrom(src => src.Gender.ToString()))
                .ForMember(
                    dest => dest.Age,
                    opt => opt.MapFrom(src => DateTime.Now.Year - src.DateOfBirth.Year));
            
            CreateMap<EmployeeAddDto, Employee>();
            CreateMap<EmployeeUpdateDto, Employee>();
            CreateMap<Employee, EmployeeUpdateDto>();
        }
    }
}