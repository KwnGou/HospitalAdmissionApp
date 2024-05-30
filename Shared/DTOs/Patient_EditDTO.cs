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
        public int Age { get; set; }

        [Required]
        public string Sex { get; set; } = null!;

        [Required]
        public int DiseaseId { get; set; }

        public string DiseaseName { get; set; }

        [MaxLength(int.MaxValue)]
        public string? PatientDetails { get; set; }

        [Required]
        public int Insurance { get; set; }

        public static Patient_EditDTO CreateFromDetailsDTO(Patient_DetailsDTO dto)
        {
            var result = new Patient_EditDTO() 
            { Id = dto.Id,
            PatientIdentityCard = dto.PatientIdentityCard,
            Name = dto.Name,
            Surname = dto.Surname,
            Age = dto.Age,
            Sex = dto.Sex,
            Insurance = dto.Insurance,
            PatientDetails = dto.PatientDetails,
            DiseaseId = dto.DiseaseId,
            DiseaseName = dto.DiseaseName
            };

            return result;
        }
    }
}
