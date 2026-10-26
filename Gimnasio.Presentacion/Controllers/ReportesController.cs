using Gimnasio.Negocio.Servicios;
using Gimnasio.Presentacion.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gimnasio.Presentacion.Controllers;

public class ReportesController : Controller
{
    private readonly ReporteService _reporteService;

    public ReportesController(ReporteService reporteService)
    {
        _reporteService = reporteService;
    }

    [HttpGet]
    public IActionResult Ingresos(DateTime? fechaInicio, DateTime? fechaFin)
    {
        var rango = ObtenerRango(fechaInicio, fechaFin);
        var modelo = CrearModelo(rango.FechaInicio, rango.FechaFin);
        modelo.Ingresos = _reporteService.IngresosPorRango(rango.FechaInicio, rango.FechaFin)
            .Select(pago => new IngresoReporteFilaViewModel
            {
                FechaPago = pago.FechaPago,
                Cliente = pago.Membresia.Cliente.Nombre,
                Plan = pago.Membresia.Plan.Nombre,
                MetodoPago = pago.MetodoPago,
                Monto = pago.Monto
            })
            .ToList();
        return View(modelo);
    }

    [HttpGet]
    public IActionResult MembresiasActivas()
    {
        var fecha = DateTime.Today;
        var modelo = CrearModelo(fecha, fecha);
        modelo.Membresias = _reporteService.MembresiasActivas(fecha)
            .Select(membresia => new MembresiaReporteFilaViewModel
            {
                Cliente = membresia.Cliente.Nombre,
                Documento = membresia.Cliente.Documento,
                Plan = membresia.Plan.Nombre,
                FechaInicio = membresia.FechaInicio,
                FechaFin = membresia.FechaFin,
                Estado = membresia.Estado
            })
            .ToList();
        return View(modelo);
    }

    [HttpGet]
    public IActionResult Asistencia(DateTime? fechaInicio, DateTime? fechaFin)
    {
        var rango = ObtenerRango(fechaInicio, fechaFin);
        var modelo = CrearModelo(rango.FechaInicio, rango.FechaFin);
        modelo.Asistencias = _reporteService.AsistenciaPorRango(rango.FechaInicio, rango.FechaFin)
            .Select(asistencia => new AsistenciaReporteFilaViewModel
            {
                FechaHoraRegistro = asistencia.FechaHoraRegistro,
                Cliente = asistencia.Cliente.Nombre,
                NombreClase = asistencia.Clase.Nombre
            })
            .ToList();
        return View(modelo);
    }

    private static ReporteViewModel CrearModelo(DateTime fechaInicio, DateTime fechaFin) => new()
    {
        FechaInicio = fechaInicio,
        FechaFin = fechaFin
    };

    private static (DateTime FechaInicio, DateTime FechaFin) ObtenerRango(DateTime? fechaInicio, DateTime? fechaFin)
    {
        var fin = (fechaFin ?? DateTime.Today).Date;
        var inicio = (fechaInicio ?? new DateTime(fin.Year, fin.Month, 1)).Date;
        var finInclusivo = fin.AddDays(1).AddTicks(-1);
        return (inicio, finInclusivo);
    }
}
