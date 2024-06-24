using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAdmissionApp.Shared.DTOs
{
    public class Patient_EditDTO
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
        public int Sex { get; set; }

        [MaxLength(int.MaxValue)]
        public string? PatientDetails { get; set; }

        [Required]
        public int Insurance { get; set; }
        
        public static Patient_EditDTO CreateFromDetails(Patient_DetailsDTO dto)
        {
            var editDto = new Patient_EditDTO 
            { Id = dto.Id,
                PatientIdentityCard = dto.PatientIdentityCard,
                Name = dto.Name,
                Surname = dto.Surname,            
                DateOfBirth = dto.DateOfBirth,
                Age = dto.Age,
                Sex = dto.Sex,
                PatientDetails = dto.PatientDetails,
                Insurance = dto.Insurance,

            };

            return editDto;
        }


    }
}
