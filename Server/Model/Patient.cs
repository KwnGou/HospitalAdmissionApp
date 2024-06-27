using System;
using System.Collections.Generic;

namespace HospitalAdmissionApp.Server.Model;

public partial class Patient
{
    public int Id { get; set; }

    public string PatientIdentityCard { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    public int Sex { get; set; }

    public string? PatientDetails { get; set; }

    public int Insurance { get; set; }

    public virtual ICollection<Slot> Slots { get; set; } = new List<Slot>();
}
