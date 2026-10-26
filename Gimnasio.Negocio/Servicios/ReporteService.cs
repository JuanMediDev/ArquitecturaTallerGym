using Gimnasio.Negocio.Entidades;
using Gimnasio.Negocio.Excepciones;
using Gimnasio.Negocio.Interfaces;

namespace Gimnasio.Negocio.Servicios;

public class ReporteService
{
    private readonly IPagoRepositorio _pagoRepositorio;
    private readonly IMembresiaRepositorio _membresiaRepositorio;
    private readonly IAsistenciaRepositorio _asistenciaRepositorio;

    public ReporteService(
        IPagoRepositorio pagoRepositorio,
        IMembresiaRepositorio membresiaRepositorio,
        IAsistenciaRepositorio asistenciaRepositorio)
    {
        _pagoRepositorio = pagoRepositorio;
        _membresiaRepositorio = membresiaRepositorio;
        _asistenciaRepositorio = asistenciaRepositorio;
    }

    public IReadOnlyList<Pago> IngresosPorRango(DateTime fechaInicio, DateTime fechaFin)
    {
        ValidarRango(fechaInicio, fechaFin);
        return _pagoRepositorio.ObtenerPorRango(fechaInicio, fechaFin)
            .Where(pago => pago.Estado == EstadoPago.Registrado)
            .ToList();
    }

    public IReadOnlyList<Membresia> MembresiasActivas(DateTime? fechaReferencia = null) =>
        _membresiaRepositorio.ObtenerActivas((fechaReferencia ?? DateTime.Today).Date);

    public IReadOnlyList<Asistencia> AsistenciaPorRango(DateTime fechaInicio, DateTime fechaFin)
    {
        ValidarRango(fechaInicio, fechaFin);
        return _asistenciaRepositorio.ObtenerPorRango(fechaInicio, fechaFin);
    }

    private static void ValidarRango(DateTime fechaInicio, DateTime fechaFin)
    {
        if (fechaFin < fechaInicio)
        {
            throw new ReglaNegocioException("La fecha final debe ser igual o posterior a la fecha inicial.");
        }
    }
}
