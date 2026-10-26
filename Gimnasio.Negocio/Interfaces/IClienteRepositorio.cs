using Gimnasio.Negocio.Entidades;

namespace Gimnasio.Negocio.Interfaces;

public interface IClienteRepositorio
{
    void Agregar(Cliente cliente);
    Cliente? ObtenerPorId(int id);
    IReadOnlyList<Cliente> ObtenerTodos();
    void Actualizar(Cliente cliente);
}
