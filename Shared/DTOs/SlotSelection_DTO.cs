using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAdmissionApp.Shared.DTOs
{
    public class SlotSelection_DTO
    {
        public int PatientId { get; set; }

        public int BedId { get; set; }

        public int DiseaseId { get; set; }
    }
}
