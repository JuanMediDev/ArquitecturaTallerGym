using Gimnasio.Negocio.Entidades;

namespace Gimnasio.Negocio.Interfaces;

public interface IPagoRepositorio
{
    void Agregar(Pago pago);
    void Anular(int id);
    IReadOnlyList<Pago> ObtenerPorMembresia(int membresiaId);
    IReadOnlyList<Pago> ObtenerPorCliente(int clienteId);
    IReadOnlyList<Pago> ObtenerPorRango(DateTime fechaInicio, DateTime fechaFin);
    bool ExisteRegistradoEnMembresia(int membresiaId);
}
