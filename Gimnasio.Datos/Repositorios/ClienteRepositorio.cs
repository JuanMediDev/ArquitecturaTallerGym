using Gimnasio.Negocio.Entidades;
using Gimnasio.Negocio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Datos.Repositorios;

public class ClienteRepositorio : IClienteRepositorio
{
    private readonly GimnasioDbContext _contexto;

    public ClienteRepositorio(GimnasioDbContext contexto) => _contexto = contexto;

    public void Agregar(Cliente cliente)
    {
        _contexto.Clientes.Add(cliente);
        _contexto.SaveChanges();
    }

    public Cliente? ObtenerPorId(int id) => _contexto.Clientes
        .AsNoTracking()
        .Include(cliente => cliente.Membresias)
            .ThenInclude(membresia => membresia.Plan)
        .Include(cliente => cliente.Membresias)
            .ThenInclude(membresia => membresia.Pagos)
        .FirstOrDefault(cliente => cliente.Id == id);

    public IReadOnlyList<Cliente> ObtenerTodos() => _contexto.Clientes
        .AsNoTracking()
        .OrderBy(cliente => cliente.Nombre)
        .ToList();

    public void Actualizar(Cliente cliente)
    {
        _contexto.Entry(cliente).State = EntityState.Modified;
        _contexto.SaveChanges();
    }
}
