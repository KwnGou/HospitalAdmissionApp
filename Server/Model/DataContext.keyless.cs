using HospitalAdmissionApp.Shared.DTOs;
using Microsoft.EntityFrameworkCore;


namespace HospitalAdmissionApp.Server.Model
{
    public partial class DataContext: DbContext
    {
		public virtual DbSet<PatientSelection_DTO> SelectablePatients { get; set; }

	}
}
