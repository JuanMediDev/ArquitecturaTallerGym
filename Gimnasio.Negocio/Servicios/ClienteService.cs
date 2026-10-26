using Gimnasio.Negocio.Entidades;
using Gimnasio.Negocio.Excepciones;
using Gimnasio.Negocio.Interfaces;

namespace Gimnasio.Negocio.Servicios;

public class ClienteService
{
    private readonly IClienteRepositorio _clienteRepositorio;

    public ClienteService(IClienteRepositorio clienteRepositorio)
    {
        _clienteRepositorio = clienteRepositorio;
    }

    public void Registrar(Cliente cliente)
    {
        _clienteRepositorio.Agregar(cliente);
    }

    public IReadOnlyList<Cliente> Consultar() => _clienteRepositorio.ObtenerTodos();

    public Cliente Consultar(int id) => _clienteRepositorio.ObtenerPorId(id)
        ?? throw new ReglaNegocioException($"No se encontró el cliente con identificador {id}.");

    public void Actualizar(Cliente cliente)
    {
        if (_clienteRepositorio.ObtenerPorId(cliente.Id) is null)
        {
            throw new ReglaNegocioException($"No se encontró el cliente con identificador {cliente.Id}.");
        }

        _clienteRepositorio.Actualizar(cliente);
    }

    public void CambiarEstado(int id, EstadoCliente estado)
    {
        var cliente = Consultar(id);
        cliente.Estado = estado;
        _clienteRepositorio.Actualizar(cliente);
    }
}
