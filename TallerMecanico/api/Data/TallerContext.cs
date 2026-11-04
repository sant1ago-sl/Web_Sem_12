using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TallerMecanico.Api.Models;

namespace TallerMecanico.Api.Data;

public partial class TallerContext : DbContext
{
    public TallerContext(DbContextOptions<TallerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Reparacion> Reparaciones { get; set; }

    public virtual DbSet<Vehiculo> Vehiculos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Reparacion>(entity =>
        {
            entity.Property(e => e.Estado).HasDefaultValue("Pendiente", "DF_Reparaciones_Estado");
            entity.Property(e => e.Fecha).HasDefaultValueSql("(CONVERT([date],getdate()))", "DF_Reparaciones_Fecha");

            entity.HasOne(d => d.Vehiculo).WithMany(p => p.Reparaciones)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reparaciones_Vehiculos");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

