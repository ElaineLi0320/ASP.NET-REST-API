using Routine.Api.Entities;
using Routine.Api.Models;

namespace Routine.Api.Profile
{
    // Class definition for mapping profiles using AutoMapper
    public class CompanyProfile: AutoMapper.Profile
    {
        // constructor
        public CompanyProfile()
        {
            // Creating a mapping configuration from Company entity to CompanyDto model
            CreateMap<Company, CompanyDto>()
                .ForMember(
                    dest => dest.CompanyName,
                    opt => opt.MapFrom(src => src.Name));
            
            CreateMap<CompanyAddDto, Company>();
        }
    }
}