using Gimnasio.Negocio.Entidades;
using Gimnasio.Negocio.Excepciones;
using Gimnasio.Negocio.Interfaces;

namespace Gimnasio.Negocio.Servicios;

public class PagoService
{
    private readonly IMembresiaRepositorio _membresiaRepositorio;
    private readonly IPagoRepositorio _pagoRepositorio;

    public PagoService(IMembresiaRepositorio membresiaRepositorio, IPagoRepositorio pagoRepositorio)
    {
        _membresiaRepositorio = membresiaRepositorio;
        _pagoRepositorio = pagoRepositorio;
    }

    public Pago RegistrarPago(int membresiaId, decimal monto, string metodoPago, DateTime? fechaPago = null)
    {
        if (_membresiaRepositorio.ObtenerPorId(membresiaId) is null)
        {
            throw new ReglaNegocioException($"No se encontró la membresía con identificador {membresiaId}.");
        }

        var pago = new Pago
        {
            MembresiaId = membresiaId,
            Monto = monto,
            FechaPago = fechaPago ?? DateTime.Now,
            MetodoPago = metodoPago,
            Estado = EstadoPago.Registrado
        };

        _pagoRepositorio.Agregar(pago);
        return pago;
    }

    public IReadOnlyList<Pago> HistorialDeCliente(int clienteId) => _pagoRepositorio.ObtenerPorCliente(clienteId);

    public bool ClienteAlDia(int clienteId)
    {
        var fecha = DateTime.Today;
        return _membresiaRepositorio.ObtenerPorCliente(clienteId)
            .Any(membresia => membresia.Estado == EstadoMembresia.Activa
                && membresia.FechaFin.Date >= fecha
                && _pagoRepositorio.ExisteRegistradoEnMembresia(membresia.Id));
    }

    public void AnularPago(int id)
    {
        _pagoRepositorio.Anular(id);
    }
}
