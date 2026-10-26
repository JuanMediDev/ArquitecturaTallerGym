using Gimnasio.Negocio.Entidades;
using Gimnasio.Negocio.Servicios;
using Gimnasio.Presentacion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gimnasio.Presentacion.Controllers;

public class ReservasController : Controller
{
    private readonly ReservaService _reservaService;
    private readonly ClienteService _clienteService;
    private readonly ClaseService _claseService;

    public ReservasController(
        ReservaService reservaService,
        ClienteService clienteService,
        ClaseService claseService)
    {
        _reservaService = reservaService;
        _clienteService = clienteService;
        _claseService = claseService;
    }

    [HttpGet]
    public IActionResult Reservar(int? claseId)
    {
        var modelo = new ReservaFormViewModel { ClaseId = claseId ?? 0 };
        CargarOpciones(modelo);
        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Reservar(ReservaFormViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            CargarOpciones(modelo);
            return View(modelo);
        }

        _reservaService.Reservar(modelo.ClienteId, modelo.ClaseId);
        TempData["Mensaje"] = "La reserva se registró correctamente.";
        return RedirectToAction(nameof(PorClase), new { claseId = modelo.ClaseId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Cancelar(int id, int claseId)
    {
        _reservaService.Cancelar(id);
        TempData["Mensaje"] = "La reserva se canceló correctamente.";
        return RedirectToAction(nameof(PorClase), new { claseId });
    }

    [HttpGet]
    public IActionResult PorClase(int? claseId)
    {
        if (!claseId.HasValue)
        {
            return View("SeleccionarClase", new ReservaClaseSeleccionViewModel
            {
                Clases = _claseService.ListarProgramacion()
                    .Select(clase => new SelectListItem(
                        $"{clase.Nombre} · {clase.FechaHoraInicio:dd/MM/yyyy HH:mm}",
                        clase.Id.ToString()))
                    .ToList()
            });
        }

        var claseSeleccionada = _claseService.ListarProgramacion()
            .FirstOrDefault(clase => clase.Id == claseId.Value);
        if (claseSeleccionada is null)
        {
            return NotFound();
        }

        var reservas = _reservaService.ConsultarReservasDeClase(claseId.Value);
        var asistencias = _reservaService.ConsultarAsistenciaPorClase(claseId.Value)
            .Select(asistencia => asistencia.ClienteId)
            .ToHashSet();
        var modelo = new ReservaClaseViewModel
        {
            ClaseId = claseSeleccionada.Id,
            NombreClase = claseSeleccionada.Nombre,
            NombreInstructor = claseSeleccionada.Instructor.Nombre,
            FechaHoraInicio = claseSeleccionada.FechaHoraInicio,
            FechaHoraFin = claseSeleccionada.FechaHoraFin,
            CupoMaximo = claseSeleccionada.CupoMaximo,
            ReservasActivas = reservas.Count(reserva => reserva.Estado == EstadoReserva.Activa),
            Reservas = reservas.Select(reserva => new ReservaFilaViewModel
            {
                Id = reserva.Id,
                ClienteId = reserva.ClienteId,
                NombreCliente = reserva.Cliente.Nombre,
                FechaReserva = reserva.FechaReserva,
                Estado = reserva.Estado,
                TieneAsistencia = asistencias.Contains(reserva.ClienteId)
            }).ToList()
        };

        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RegistrarAsistencia(int clienteId, int claseId)
    {
        _reservaService.RegistrarAsistencia(clienteId, claseId);
        TempData["Mensaje"] = "La asistencia se registró correctamente.";
        return RedirectToAction(nameof(PorClase), new { claseId });
    }

    private void CargarOpciones(ReservaFormViewModel modelo)
    {
        modelo.Clientes = _clienteService.Consultar()
            .Select(cliente => new SelectListItem(cliente.Nombre, cliente.Id.ToString(), cliente.Id == modelo.ClienteId))
            .ToList();
        modelo.Clases = _claseService.ListarProgramacion()
            .Where(clase => clase.Estado == EstadoClase.Programada)
            .Select(clase => new SelectListItem(
                $"{clase.Nombre} · {clase.FechaHoraInicio:dd/MM/yyyy HH:mm}",
                clase.Id.ToString(),
                clase.Id == modelo.ClaseId))
                            .ToList();
                }
            }
