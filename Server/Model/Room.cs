using System;
using System.Collections.Generic;

namespace HospitalAdmissionApp.Server.Model;

public partial class Room
{
    public int Id { get; set; }

    public string RoomNumber { get; set; } = null!;

    public int ClinicId { get; set; }

    public virtual ICollection<Bed> Beds { get; set; } = new List<Bed>();

    public virtual Clinic Clinic { get; set; } = null!;
}
