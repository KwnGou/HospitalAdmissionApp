using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAdmissionApp.Shared.DTOs
{
    public class Slot_GridDTO
    {
        [Key]
        public int Id { get; set; }

        public int PatientId { get; set; }

        public int BedId { get; set; }

        public string BedInfo { get; set; }

        public string PatientName {  get; set; }

        public string PatientSurname { get; set;}

        public string PatientDisease { get; set;}

        [Required]
        public DateTimeOffset AdmissionDate { get; set; }
        [Required]
        public DateTimeOffset ReleaseDate { get; set; }
    }
}
