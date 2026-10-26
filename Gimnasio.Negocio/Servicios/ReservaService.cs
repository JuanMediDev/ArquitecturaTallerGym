using Gimnasio.Negocio.Entidades;
using Gimnasio.Negocio.Excepciones;
using Gimnasio.Negocio.Interfaces;

namespace Gimnasio.Negocio.Servicios;

public class ReservaService
{
    private readonly IClienteRepositorio _clienteRepositorio;
    private readonly IMembresiaRepositorio _membresiaRepositorio;
    private readonly IPagoRepositorio _pagoRepositorio;
    private readonly IClaseRepositorio _claseRepositorio;
    private readonly IReservaRepositorio _reservaRepositorio;
    private readonly IAsistenciaRepositorio _asistenciaRepositorio;

    public ReservaService(
        IClienteRepositorio clienteRepositorio,
        IMembresiaRepositorio membresiaRepositorio,
        IPagoRepositorio pagoRepositorio,
        IClaseRepositorio claseRepositorio,
        IReservaRepositorio reservaRepositorio,
        IAsistenciaRepositorio asistenciaRepositorio)
    {
        _clienteRepositorio = clienteRepositorio;
        _membresiaRepositorio = membresiaRepositorio;
        _pagoRepositorio = pagoRepositorio;
        _claseRepositorio = claseRepositorio;
        _reservaRepositorio = reservaRepositorio;
        _asistenciaRepositorio = asistenciaRepositorio;
    }

    public Reserva Reservar(int clienteId, int claseId)
    {
        var cliente = _clienteRepositorio.ObtenerPorId(clienteId)
            ?? throw new ReglaNegocioException($"No se encontró el cliente con identificador {clienteId}.");
        var clase = _claseRepositorio.ObtenerPorId(claseId)
            ?? throw new ReglaNegocioException($"No se encontró la clase con identificador {claseId}.");

        if (cliente.Estado != EstadoCliente.Activo)
        {
            throw new ReglaNegocioException("Solo los clientes activos pueden reservar clases.");
        }

        if (!TieneAcceso(clienteId, DateTime.Today))
        {
            throw new ReglaNegocioException("El cliente necesita una membresía activa vigente y con pagos registrados.");
        }

        if (_reservaRepositorio.ExisteActiva(clienteId, claseId))
        {
            throw new ReglaNegocioException("El cliente ya tiene una reserva activa para esta clase.");
        }

        if (_reservaRepositorio.ContarActivasPorClase(claseId) >= clase.CupoMaximo)
        {
            throw new ReglaNegocioException("La clase alcanzó su cupo máximo de reservas.");
        }

        var reserva = new Reserva
        {
            ClienteId = clienteId,
            ClaseId = claseId,
            FechaReserva = DateTime.Now,
            Estado = EstadoReserva.Activa
        };

        _reservaRepositorio.Agregar(reserva);
        return reserva;
    }

    public void Cancelar(int reservaId)
    {
        var reserva = _reservaRepositorio.ObtenerPorId(reservaId)
            ?? throw new ReglaNegocioException($"No se encontró la reserva con identificador {reservaId}.");

        if (reserva.Estado != EstadoReserva.Activa)
        {
            throw new ReglaNegocioException("La reserva ya está cancelada.");
        }

        reserva.Estado = EstadoReserva.Cancelada;
        _reservaRepositorio.Actualizar(reserva);
    }

    public IReadOnlyList<Reserva> ConsultarReservasDeClase(int claseId)
    {
        if (_claseRepositorio.ObtenerPorId(claseId) is null)
        {
            throw new ReglaNegocioException($"No se encontró la clase con identificador {claseId}.");
        }

        return _reservaRepositorio.ObtenerPorClase(claseId);
    }

    public Asistencia RegistrarAsistencia(int clienteId, int claseId)
    {
        var clase = _claseRepositorio.ObtenerPorId(claseId)
            ?? throw new ReglaNegocioException($"No se encontró la clase con identificador {claseId}.");

        if (clase.Estado != EstadoClase.Programada)
        {
            throw new ReglaNegocioException("Solo se puede registrar asistencia a una clase programada.");
        }

        if (!_reservaRepositorio.ExisteActiva(clienteId, claseId))
        {
            throw new ReglaNegocioException("El cliente no tiene una reserva activa para esta clase.");
        }

        var asistencia = new Asistencia
        {
            ClienteId = clienteId,
            ClaseId = claseId,
            FechaHoraRegistro = DateTime.Now
        };

        _asistenciaRepositorio.Agregar(asistencia);
        return asistencia;
    }

    public IReadOnlyList<Asistencia> ConsultarAsistenciaPorCliente(int clienteId) =>
        _asistenciaRepositorio.ObtenerPorCliente(clienteId);

    public IReadOnlyList<Asistencia> ConsultarAsistenciaPorClase(int claseId) =>
        _asistenciaRepositorio.ObtenerPorClase(claseId);

    private bool TieneAcceso(int clienteId, DateTime fecha)
    {
        return _membresiaRepositorio.ObtenerPorCliente(clienteId)
            .Any(membresia => membresia.Estado == EstadoMembresia.Activa
                && membresia.FechaFin.Date >= fecha.Date
                && _pagoRepositorio.ExisteRegistradoEnMembresia(membresia.Id));
    }
}
