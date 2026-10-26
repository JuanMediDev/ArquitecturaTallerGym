using Gimnasio.Negocio.Entidades;
using Gimnasio.Negocio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Datos.Repositorios;

public class MembresiaRepositorio : IMembresiaRepositorio
{
    private readonly GimnasioDbContext _contexto;

    public MembresiaRepositorio(GimnasioDbContext contexto) => _contexto = contexto;

    public void Agregar(Membresia membresia)
    {
        _contexto.Membresias.Add(membresia);
        _contexto.SaveChanges();
    }

    public Membresia? ObtenerPorId(int id) => _contexto.Membresias
        .AsNoTracking()
        .Include(membresia => membresia.Plan)
        .Include(membresia => membresia.Pagos)
        .FirstOrDefault(membresia => membresia.Id == id);

    public IReadOnlyList<Membresia> ObtenerPorCliente(int clienteId) => _contexto.Membresias
        .AsNoTracking()
        .Include(membresia => membresia.Plan)
        .Include(membresia => membresia.Pagos)
        .Where(membresia => membresia.ClienteId == clienteId)
        .OrderByDescending(membresia => membresia.FechaInicio)
        .ToList();

    public IReadOnlyList<Membresia> ObtenerActivas(DateTime fecha) => _contexto.Membresias
        .AsNoTracking()
        .Include(membresia => membresia.Cliente)
        .Include(membresia => membresia.Plan)
        .Where(membresia => membresia.Estado == EstadoMembresia.Activa && membresia.FechaFin >= fecha)
        .OrderBy(membresia => membresia.FechaFin)
        .ToList();

    public IReadOnlyList<Membresia> ObtenerVencidas(DateTime fecha) => _contexto.Membresias
        .AsNoTracking()
        .Where(membresia => membresia.Estado == EstadoMembresia.Activa && membresia.FechaFin < fecha)
        .ToList();

    public void Actualizar(Membresia membresia)
    {
        _contexto.Membresias.Update(membresia);
        _contexto.SaveChanges();
    }
}
