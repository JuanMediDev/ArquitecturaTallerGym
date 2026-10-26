using Gimnasio.Negocio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Datos;

public class GimnasioDbContext : DbContext
{
    public GimnasioDbContext(DbContextOptions<GimnasioDbContext> opciones)
        : base(opciones)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<PlanMembresia> PlanesMembresia => Set<PlanMembresia>();
    public DbSet<Membresia> Membresias => Set<Membresia>();
    public DbSet<Pago> Pagos => Set<Pago>();
    public DbSet<Instructor> Instructores => Set<Instructor>();
    public DbSet<Clase> Clases => Set<Clase>();
    public DbSet<Reserva> Reservas => Set<Reserva>();
    public DbSet<Asistencia> Asistencias => Set<Asistencia>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entidad =>
        {
            entidad.HasKey(cliente => cliente.Id);
            entidad.Property(cliente => cliente.Nombre).HasMaxLength(120).IsRequired();
            entidad.Property(cliente => cliente.Documento).HasMaxLength(30).IsRequired();
            entidad.Property(cliente => cliente.Correo).HasMaxLength(150).IsRequired();
            entidad.Property(cliente => cliente.Telefono).HasMaxLength(30).IsRequired();
            entidad.Property(cliente => cliente.Estado).HasConversion<string>().HasMaxLength(20);
        });

        modelBuilder.Entity<PlanMembresia>(entidad =>
        {
            entidad.HasKey(plan => plan.Id);
            entidad.Property(plan => plan.Nombre).HasMaxLength(100).IsRequired();
            entidad.Property(plan => plan.Beneficios).HasMaxLength(500).IsRequired();
            entidad.Property(plan => plan.Precio).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Membresia>(entidad =>
        {
            entidad.HasKey(membresia => membresia.Id);
            entidad.Property(membresia => membresia.Estado).HasConversion<string>().HasMaxLength(20);
            entidad.HasOne(membresia => membresia.Cliente)
                .WithMany(cliente => cliente.Membresias)
                .HasForeignKey(membresia => membresia.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
            entidad.HasOne(membresia => membresia.Plan)
                .WithMany(plan => plan.Membresias)
                .HasForeignKey(membresia => membresia.PlanId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Pago>(entidad =>
        {
            entidad.HasKey(pago => pago.Id);
            entidad.Property(pago => pago.Monto).HasPrecision(18, 2);
            entidad.Property(pago => pago.MetodoPago).HasMaxLength(50).IsRequired();
            entidad.Property(pago => pago.Estado).HasConversion<string>().HasMaxLength(20);
            entidad.HasOne(pago => pago.Membresia)
                .WithMany(membresia => membresia.Pagos)
                .HasForeignKey(pago => pago.MembresiaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Instructor>(entidad =>
        {
            entidad.HasKey(instructor => instructor.Id);
            entidad.Property(instructor => instructor.Nombre).HasMaxLength(120).IsRequired();
            entidad.Property(instructor => instructor.Especialidad).HasMaxLength(100).IsRequired();
            entidad.HasData(InstructoresDemostracion.Datos);
        });

        modelBuilder.Entity<Clase>(entidad =>
        {
            entidad.HasKey(clase => clase.Id);
            entidad.Property(clase => clase.Nombre).HasMaxLength(100).IsRequired();
            entidad.Property(clase => clase.Estado).HasConversion<string>().HasMaxLength(20);
            entidad.HasOne(clase => clase.Instructor)
                .WithMany(instructor => instructor.Clases)
                .HasForeignKey(clase => clase.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Reserva>(entidad =>
        {
            entidad.HasKey(reserva => reserva.Id);
            entidad.Property(reserva => reserva.Estado).HasConversion<string>().HasMaxLength(20);
            entidad.HasIndex(reserva => new { reserva.ClienteId, reserva.ClaseId })
                .IsUnique()
                .HasFilter("[Estado] = N'Activa'");
            entidad.HasOne(reserva => reserva.Cliente)
                .WithMany(cliente => cliente.Reservas)
                .HasForeignKey(reserva => reserva.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
            entidad.HasOne(reserva => reserva.Clase)
                .WithMany(clase => clase.Reservas)
                .HasForeignKey(reserva => reserva.ClaseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Asistencia>(entidad =>
        {
            entidad.HasKey(asistencia => asistencia.Id);
            entidad.HasOne(asistencia => asistencia.Cliente)
                .WithMany(cliente => cliente.Asistencias)
                .HasForeignKey(asistencia => asistencia.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
            entidad.HasOne(asistencia => asistencia.Clase)
                .WithMany(clase => clase.Asistencias)
                .HasForeignKey(asistencia => asistencia.ClaseId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
