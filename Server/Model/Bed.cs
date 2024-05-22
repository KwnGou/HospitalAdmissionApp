using System;
using System.Collections.Generic;

namespace HospitalAdmissionApp.Server.Model;

public partial class Bed
{
    public int Id { get; set; }

    public int RoomId { get; set; }

    public string BedInfo { get; set; } = null!;

    public virtual Room Room { get; set; } = null!;

    public virtual ICollection<Slot> Slots { get; set; } = new List<Slot>();
}
