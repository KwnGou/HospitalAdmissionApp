using HospitalAdmissionApp.Shared.DTOs;
using Microsoft.EntityFrameworkCore;


namespace HospitalAdmissionApp.Server.Model
{
    public partial class DataContext : DbContext
    {
        public virtual DbSet<PatientSelection_DTO> SelectablePatients { get; set; }

        public virtual DbSet<Bed_GridDTO> SelectableBeds { get; set; }

        public virtual DbSet<RoomInfo_DTO> RoomUsedBedsInfo { get; set; }

        public virtual DbSet<Patient_GridDTO> PatientsList { get; set; }

        public virtual DbSet<RoomDetails_DTO> RoomDetailsList { get; set; }

    }
}
