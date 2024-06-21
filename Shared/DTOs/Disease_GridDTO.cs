using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAdmissionApp.Shared.DTOs
{
    public class Disease_GridDTO
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClinicId { get; set; }

        public string? ClinicName {  get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

    }
}
