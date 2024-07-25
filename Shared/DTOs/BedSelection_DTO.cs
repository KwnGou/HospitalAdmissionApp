using System.ComponentModel.DataAnnotations;

namespace HospitalAdmissionApp.Shared.DTOs
{
    public class BedSelection_DTO
    {
        [Key]
        public int BedId { get; set; }

        public int RoomId { get; set; }

        public string RoomNumber { get; set; }

        public string BedInfo { get; set; } = null!;

    }
}
