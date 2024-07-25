using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HospitalAdmissionApp.Shared.DTOs
{
    public class Slot_GridDTO
    {
        [Key]
        public int Id { get; set; }

        public int PatientId { get; set; }

        public int BedId { get; set; }

        public int DiseaseId { get; set; }

        public string RoomNumber { get; set; }

        public string BedInfo { get; set; }

        public string PatientName {  get; set; }

        public string PatientSurname { get; set;}

        public string DiseaseName { get; set;}

        public DateTimeOffset AdmissionDate { get; set; }

        public DateTimeOffset? ReleaseDate { get; set; }

    }
}
