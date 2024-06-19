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

            CreateMap<Room, Room_GridDTO>()
                .ForMember(dest => dest.ClinicName, opt => opt.MapFrom(src => src.Clinic.Name))
                .ReverseMap();

            CreateMap<Bed, Bed_GridDTO>()
                .ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => src.Room.RoomNumber))
                .ReverseMap();

            CreateMap<Slot, Slot_GridDTO>()
                .ForMember(dest => dest.BedInfo, opt => opt.MapFrom(src => src.Bed.BedInfo))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.Name))
                .ForMember(dest => dest.PatientSurname, opt => opt.MapFrom(src => src.Patient.Surname))
                .ForMember(dest => dest.DiseaseName, opt => opt.MapFrom(src => src.Disease.Id))
                .ReverseMap();

            CreateMap<Patient, Patient_DetailsDTO>()
                .ReverseMap();

            CreateMap<Patient, Patient_EditDTO>()
                .ReverseMap();

            CreateMap<Patient, Patient_GridDTO>()
                .ReverseMap();
        }
    }
}
