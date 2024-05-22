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
        }
    }
}
