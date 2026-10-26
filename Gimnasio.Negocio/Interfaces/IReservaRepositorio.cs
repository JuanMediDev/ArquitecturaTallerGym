using Gimnasio.Negocio.Entidades;

namespace Gimnasio.Negocio.Interfaces;

public interface IReservaRepositorio
{
    void Agregar(Reserva reserva);
    Reserva? ObtenerPorId(int id);
    IReadOnlyList<Reserva> ObtenerPorClase(int claseId);
    IReadOnlyList<Reserva> ObtenerPorCliente(int clienteId);
    Reserva? ObtenerActiva(int clienteId, int claseId);
    int ContarActivasPorClase(int claseId);
    bool ExisteActiva(int clienteId, int claseId);
    void Actualizar(Reserva reserva);
}
