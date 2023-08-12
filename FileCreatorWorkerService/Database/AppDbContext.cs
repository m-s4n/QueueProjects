using System;
using System.Collections.Generic;
using FileCreatorWorkerService.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileCreatorWorkerService.Database;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Urun> Uruns { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Urun>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("urun_pkey");

            entity.ToTable("urun");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Ad)
                .HasMaxLength(100)
                .HasColumnName("ad");
            entity.Property(e => e.IsAktif)
                .IsRequired()
                .HasDefaultValueSql("true")
                .HasColumnName("is_aktif");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Numara)
                .HasMaxLength(100)
                .HasColumnName("numara");
            entity.Property(e => e.OpTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("op_time");
            entity.Property(e => e.Renk)
                .HasMaxLength(30)
                .HasColumnName("renk");
            entity.Property(e => e.Tur)
                .HasDefaultValueSql("1")
                .HasColumnName("tur");
            entity.Property(e => e.Ucret).HasColumnName("ucret");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
