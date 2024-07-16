using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAdmissionApp.Shared.DTOs
{
    public class PatientSelection_DTO
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Surname { get; set; } = null!;

        public string PatientIdentityCard { get; set; } = null!;
    }
}
