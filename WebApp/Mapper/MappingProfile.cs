using AutoMapper;
using ClassLibrary.Models;
using WebApp.Controllers;
using WebApp.Models;

namespace WebApp.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        { 
            CreateMap<MedicalFile,MedicalFileVM>().ReverseMap();
            CreateMap<MedicalRecord, MedicalRecordVM>().ReverseMap();
            CreateMap<Examination, ExaminationVM>().ReverseMap();
            CreateMap<Patient, PatientVM>().ReverseMap();
            CreateMap<User, UserLoginVM>().ReverseMap();
            CreateMap<Prescription, PrescriptionVM>().ReverseMap();
        }
    }
}
