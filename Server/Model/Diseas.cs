using System;
using System.Collections.Generic;

namespace HospitalAdmissionApp.Server.Model;

public partial class Diseas
{
    public int Id { get; set; }

    public int ClinicId { get; set; }

    public string Name { get; set; } = null!;

    public virtual Clinic Clinic { get; set; } = null!;

    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
}
