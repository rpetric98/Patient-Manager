using AutoMapper;
using ClassLibrary.Models;
using WebApi.Dtos;

namespace WebApi.Mapping
{
    public class MappingProfile : Profile
    {
        protected MappingProfile() 
        {
            CreateMap<Patient, PatientDto>();
            CreateMap<User , UserDto>();
        }
    }
}
