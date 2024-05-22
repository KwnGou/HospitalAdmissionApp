using System;
using System.Collections.Generic;

namespace HospitalAdmissionApp.Server.Model;

public partial class Clinic
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Diseas> Diseas { get; set; } = new List<Diseas>();

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
