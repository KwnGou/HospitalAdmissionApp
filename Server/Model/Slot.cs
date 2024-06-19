using System;
using System.Collections.Generic;

namespace HospitalAdmissionApp.Server.Model;

public partial class Slot
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public int BedId { get; set; }

    public int DiseaseId { get; set; }

    public DateTimeOffset AdmissionDate { get; set; }

    public DateTimeOffset? ReleaseDate { get; set; }

    public virtual Bed Bed { get; set; } = null!;

    public virtual Diseas Disease { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;
}
