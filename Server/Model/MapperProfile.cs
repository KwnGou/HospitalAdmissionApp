using AutoMapper;
using HospitalAdmissionApp.Shared.DTOs;

namespace HospitalAdmissionApp.Server.Model
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Clinic, Clinic_GridDTO>()
                .ReverseMap();

            CreateMap<Diseas, Disease_GridDTO>()
                .ForMember(dest => dest.ClinicName, opt => opt.MapFrom(src => src.Clinic.Name))
                .ReverseMap();
        }
    }
}
