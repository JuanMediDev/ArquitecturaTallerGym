using Gimnasio.Negocio.Entidades;

namespace Gimnasio.Presentacion.Models;

public class ReporteViewModel
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public IReadOnlyList<IngresoReporteFilaViewModel> Ingresos { get; set; } = Array.Empty<IngresoReporteFilaViewModel>();
    public IReadOnlyList<MembresiaReporteFilaViewModel> Membresias { get; set; } = Array.Empty<MembresiaReporteFilaViewModel>();
    public IReadOnlyList<AsistenciaReporteFilaViewModel> Asistencias { get; set; } = Array.Empty<AsistenciaReporteFilaViewModel>();
    public decimal TotalIngresos => Ingresos.Sum(ingreso => ingreso.Monto);
}

public class IngresoReporteFilaViewModel
{
    public DateTime FechaPago { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public string Plan { get; set; } = string.Empty;
    public string MetodoPago { get; set; } = string.Empty;
    public decimal Monto { get; set; }
}

public class MembresiaReporteFilaViewModel
{
    public string Cliente { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Plan { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public EstadoMembresia Estado { get; set; }
}

public class AsistenciaReporteFilaViewModel
{
    public DateTime FechaHoraRegistro { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public string NombreClase { get; set; } = string.Empty;
}
