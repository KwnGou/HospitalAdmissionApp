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

        [Required]
        [MaxLength(50)]
        public string PatientIdentityCard { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Surname { get; set; } = null!;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public int Age { get; set; }

        [Required]
        public string Sex { get; set; } = null!; 

        [MaxLength(int.MaxValue)]
        public string? PatientDetails { get; set; }

        [Required]
        public int Insurance { get; set; }

        public static Patient_DetailsDTO CreateFromDetailsDTO(Patient_EditDTO dto)
        {
            var result = new Patient_DetailsDTO()
            {
                Id = dto.Id,
                PatientIdentityCard = dto.PatientIdentityCard,
                Name = dto.Name,
                Surname = dto.Surname,
                Age = dto.Age,
                Sex = dto.Sex,
                Insurance = dto.Insurance,
                PatientDetails = dto.PatientDetails,
            };

            return result;
        }
    }
}
