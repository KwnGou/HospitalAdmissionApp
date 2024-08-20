using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace HospitalAdmissionApp.Shared.DTOs
{
    [PrimaryKey(nameof(RoomId), nameof(BedId))]
    //[Keyless]
    public class RoomDetails_DTO
    {
        public int RoomId { get; set; }

        public string? RoomNumber {  get; set; }

        public int BedId { get; set; }

        public string BedInfo { get; set; }

        public int? PatientId { get; set; }

        public string? PatientName {  get; set; }

        public string? PatientSurname {  get; set; }

        public string? DiseaseName { get; set; }

        public bool? Occupied { get; set; }
    }
}
