using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAdmissionApp.Shared.DTOs
{
    public class Patient_DetailsDTO
    {
        [Key]
        public int Id { get; set; }

        public string PatientIdentityCard { get; set; } = null!;

        public string Name { get; set; } = null!;

        [MaxLength(50)]
        public string Surname { get; set; } = null!;

        public DateTime DateOfBirth { get; set; }

        public int Age { get; set; }

        public int Sex { get; set; }
        public string SexText { get; set; }

        public int Insurance { get; set; }
        public string InsuranceText { get; set; }

        public string? PatientDetails { get; set; }

        public List<PatientHistory_GridDTO> History { get; set; }
    }
}
