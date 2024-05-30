using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAdmissionApp.Shared.DTOs
{
    public class Patient_GridDTO
    {
        [Key]
        public int Id { get; set; }
        //[Required]
        //[MaxLength(50)]
        //public string PatientIdentityCard { get; set; } = null!;
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;
        [Required]
        [MaxLength(50)]
        public string Surname { get; set; } = null!;

        public string DiseaseName { get; set; }
    }
}
