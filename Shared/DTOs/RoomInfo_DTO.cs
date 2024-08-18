using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAdmissionApp.Shared.DTOs
{
    [Keyless]
    public class RoomInfo_DTO
    {
        public string BedInfo { get; set; }
        public DateTime PatientDateOfBirth { get; set; }
        public int PatientSex { get; set; }
        public string PatientDiseaseName { get; set; }
    }
}
