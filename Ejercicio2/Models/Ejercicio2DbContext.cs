using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Ejercicio2.Models;

public partial class Ejercicio2DbContext : DbContext
{
    public Ejercicio2DbContext()
    {
    }

    public Ejercicio2DbContext(DbContextOptions<Ejercicio2DbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cuentahabiente> Cuentahabientes { get; set; }

    public virtual DbSet<Denominacione> Denominaciones { get; set; }

    public virtual DbSet<Transaccione> Transacciones { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cuentahabiente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cuentaha__3214EC074FECCF2A");

            entity.ToTable("Cuentahabiente");

            entity.Property(e => e.FechaAlta)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NombreCompleto).HasMaxLength(100);
            entity.Property(e => e.Saldo).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Denominacione>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Denomina__3214EC072B72FC6F");

            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<Transaccione>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Transacc__3214EC077C79C35D");

            entity.Property(e => e.FechaTransaccion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Monto).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TipoTransaccion).HasMaxLength(50);

            entity.HasOne(d => d.Cuentahabiente).WithMany(p => p.Transacciones)
                .HasForeignKey(d => d.CuentahabienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacci__Cuent__3A81B327");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
