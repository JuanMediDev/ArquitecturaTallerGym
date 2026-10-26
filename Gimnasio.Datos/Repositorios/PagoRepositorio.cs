using Gimnasio.Negocio.Entidades;
using Gimnasio.Negocio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Datos.Repositorios;

public class PagoRepositorio : IPagoRepositorio
{
    private readonly GimnasioDbContext _contexto;

    public PagoRepositorio(GimnasioDbContext contexto) => _contexto = contexto;

    public void Agregar(Pago pago)
    {
        _contexto.Pagos.Add(pago);
        _contexto.SaveChanges();
    }

    public void Anular(int id)
    {
        var pago = _contexto.Pagos.Find(id)
            ?? throw new KeyNotFoundException($"No se encontró el pago con identificador {id}.");

        pago.Estado = EstadoPago.Anulado;
        _contexto.SaveChanges();
    }

    public IReadOnlyList<Pago> ObtenerPorMembresia(int membresiaId) => _contexto.Pagos
        .AsNoTracking()
        .Where(pago => pago.MembresiaId == membresiaId)
        .OrderByDescending(pago => pago.FechaPago)
        .ToList();

    public IReadOnlyList<Pago> ObtenerPorCliente(int clienteId) => _contexto.Pagos
        .AsNoTracking()
        .Include(pago => pago.Membresia)
        .Where(pago => pago.Membresia.ClienteId == clienteId)
        .OrderByDescending(pago => pago.FechaPago)
        .ToList();

    public IReadOnlyList<Pago> ObtenerPorRango(DateTime fechaInicio, DateTime fechaFin) => _contexto.Pagos
        .AsNoTracking()
        .Include(pago => pago.Membresia)
            .ThenInclude(membresia => membresia.Cliente)
        .Include(pago => pago.Membresia)
            .ThenInclude(membresia => membresia.Plan)
        .Where(pago => pago.FechaPago >= fechaInicio && pago.FechaPago <= fechaFin)
        .OrderBy(pago => pago.FechaPago)
        .ToList();

    public bool ExisteRegistradoEnMembresia(int membresiaId) => _contexto.Pagos
        .Any(pago => pago.MembresiaId == membresiaId && pago.Estado == EstadoPago.Registrado);
}
