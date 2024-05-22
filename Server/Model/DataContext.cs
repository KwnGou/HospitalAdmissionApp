using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HospitalAdmissionApp.Server.Model;

public partial class DataContext : DbContext
{

    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bed> Beds { get; set; }

    public virtual DbSet<Clinic> Clinics { get; set; }

    public virtual DbSet<Diseas> Diseases { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Slot> Slots { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bed>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Table_1");

            entity.Property(e => e.BedInfo)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Room).WithMany(p => p.Beds)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Beds_Beds");
        });

        modelBuilder.Entity<Clinic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Clinic");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Diseas>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ClinicDisease");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Clinic).WithMany(p => p.Diseas)
                .HasForeignKey(d => d.ClinicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClinicDisease_Clinic");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Patient");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PatientDetails).IsUnicode(false);
            entity.Property(e => e.PatientIdentityCard)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Sex)
                .HasMaxLength(1)
                .IsUnicode(false);
            entity.Property(e => e.Surname)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Disease).WithMany(p => p.Patients)
                .HasForeignKey(d => d.DiseaseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Patient_Patient");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Room");

            entity.HasOne(d => d.Clinic).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.ClinicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Room_Room1");
        });

        modelBuilder.Entity<Slot>(entity =>
        {
            entity.HasIndex(e => new { e.PatientId, e.BedId }, "IX_Slots_Unique_Patient_Bed").IsUnique();

            entity.HasOne(d => d.Bed).WithMany(p => p.Slots)
                .HasForeignKey(d => d.BedId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Slots_Beds");

            entity.HasOne(d => d.Patient).WithMany(p => p.Slots)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Slots_Patient");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
