using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HospitalAdmissionApp.Shared.DTOs
{
    public class Room_GridDTO
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string RoomNumber { get; set; }

        public int ClinicId { get; set; }

        public string? ClinicName { get; set; }

        public override string ToString()
        {
            return $"{RoomNumber} ({ClinicName})";
        }
    }
}
