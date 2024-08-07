using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAdmissionApp.Shared.DTOs
{
    public class Bed_GridDTO
    {
        [Key]
        public int BedId { get; set; }

        public int RoomId { get; set; }

        public string RoomNumber { get; set; }

        public string BedInfo { get; set; } = null!;
    }
}
