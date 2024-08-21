using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAdmissionApp.Shared.DTOs
{
    public class RoomOverview_DTO
    {
        public int RoomId { get; set; }

        public string? RoomNumber { get; set; }

        public bool Occupied { get; set; }

        public  List<BedOverview_DTO> Beds  { get; set; }
    }

    public class BedOverview_DTO 
    {
        public int BedId { get; set; }

        public string BedInfo { get; set; }

        public int? PatientId { get; set; }

        public string? PatientFullName { get; set; }

        public string? DiseaseName { get; set; }
    }

}
