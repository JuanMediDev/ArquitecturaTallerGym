using Gimnasio.Negocio.Entidades;
using Gimnasio.Negocio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Datos.Repositorios;

public class ReservaRepositorio : IReservaRepositorio
{
    private readonly GimnasioDbContext _contexto;

    public ReservaRepositorio(GimnasioDbContext contexto) => _contexto = contexto;

    public void Agregar(Reserva reserva)
    {
        _contexto.Reservas.Add(reserva);
        _contexto.SaveChanges();
    }

    public Reserva? ObtenerPorId(int id) => _contexto.Reservas
        .AsNoTracking()
        .Include(reserva => reserva.Cliente)
        .Include(reserva => reserva.Clase)
        .FirstOrDefault(reserva => reserva.Id == id);

    public IReadOnlyList<Reserva> ObtenerPorClase(int claseId) => _contexto.Reservas
        .AsNoTracking()
        .Include(reserva => reserva.Cliente)
        .Where(reserva => reserva.ClaseId == claseId)
        .OrderBy(reserva => reserva.FechaReserva)
        .ToList();

    public IReadOnlyList<Reserva> ObtenerPorCliente(int clienteId) => _contexto.Reservas
        .AsNoTracking()
        .Include(reserva => reserva.Clase)
        .Where(reserva => reserva.ClienteId == clienteId)
        .OrderByDescending(reserva => reserva.FechaReserva)
        .ToList();

    public Reserva? ObtenerActiva(int clienteId, int claseId) => _contexto.Reservas
        .AsNoTracking()
        .FirstOrDefault(reserva => reserva.ClienteId == clienteId
            && reserva.ClaseId == claseId
            && reserva.Estado == EstadoReserva.Activa);

    public int ContarActivasPorClase(int claseId) => _contexto.Reservas
        .Count(reserva => reserva.ClaseId == claseId && reserva.Estado == EstadoReserva.Activa);

    public bool ExisteActiva(int clienteId, int claseId) => _contexto.Reservas
        .Any(reserva => reserva.ClienteId == clienteId
            && reserva.ClaseId == claseId
            && reserva.Estado == EstadoReserva.Activa);

    public void Actualizar(Reserva reserva)
    {
        _contexto.Reservas.Update(reserva);
        _contexto.SaveChanges();
    }
}
