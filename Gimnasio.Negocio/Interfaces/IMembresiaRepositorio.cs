using Gimnasio.Negocio.Entidades;

namespace Gimnasio.Negocio.Interfaces;

public interface IMembresiaRepositorio
{
    void Agregar(Membresia membresia);
    Membresia? ObtenerPorId(int id);
    IReadOnlyList<Membresia> ObtenerPorCliente(int clienteId);
    IReadOnlyList<Membresia> ObtenerActivas(DateTime fecha);
    IReadOnlyList<Membresia> ObtenerVencidas(DateTime fecha);
    void Actualizar(Membresia membresia);
}
