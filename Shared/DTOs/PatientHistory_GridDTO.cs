using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAdmissionApp.Shared.DTOs
{
    public class PatientHistory_GridDTO
    {
        [Key]
        public int Id { get; set; }

        public int BedId { get; set; }

        public int DiseaseId { get; set; }

        public string RoomNumber { get; set; }

        public string BedInfo { get; set; }

        public string DiseaseName { get; set; }

        public DateTimeOffset AdmissionDate { get; set; }

        public DateTimeOffset? ReleaseDate { get; set; }
    }
}
