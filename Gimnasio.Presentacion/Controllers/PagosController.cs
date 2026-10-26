using Gimnasio.Negocio.Servicios;
using Gimnasio.Presentacion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gimnasio.Presentacion.Controllers;

public class PagosController : Controller
{
    private readonly PagoService _pagoService;
    private readonly ClienteService _clienteService;
    private readonly MembresiaService _membresiaService;

    public PagosController(
        PagoService pagoService,
        ClienteService clienteService,
        MembresiaService membresiaService)
    {
        _pagoService = pagoService;
        _clienteService = clienteService;
        _membresiaService = membresiaService;
    }

    [HttpGet]
    public IActionResult Registrar(int? clienteId, int? membresiaId)
    {
        var modelo = new PagoFormViewModel
        {
            ClienteId = clienteId ?? 0,
            MembresiaId = membresiaId ?? 0
        };
        CargarOpciones(modelo);
        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Registrar(PagoFormViewModel modelo)
    {
        if (ModelState.IsValid && !MembresiaPerteneceACliente(modelo.MembresiaId, modelo.ClienteId))
        {
            ModelState.AddModelError(nameof(modelo.MembresiaId), "La membresía seleccionada no corresponde al cliente.");
        }

        if (!ModelState.IsValid)
        {
            CargarOpciones(modelo);
            return View(modelo);
        }

        _pagoService.RegistrarPago(modelo.MembresiaId, modelo.Monto, modelo.MetodoPago.Trim(), modelo.FechaPago);
        TempData["Mensaje"] = "El pago se registró correctamente.";
        return RedirectToAction(nameof(Historial), new { clienteId = modelo.ClienteId });
    }

    [HttpGet]
    public IActionResult Historial(int? clienteId)
    {
        var modelo = new PagoHistorialViewModel
        {
            ClienteId = clienteId,
            Clientes = _clienteService.Consultar()
                .Select(cliente => new SelectListItem(cliente.Nombre, cliente.Id.ToString(), cliente.Id == clienteId))
                .ToList()
        };

        if (clienteId.HasValue)
        {
            modelo.NombreCliente = _clienteService.Consultar(clienteId.Value).Nombre;
            modelo.Pagos = _pagoService.HistorialDeCliente(clienteId.Value)
                .Select(pago => new PagoFilaViewModel
                {
                    MembresiaId = pago.MembresiaId,
                    Monto = pago.Monto,
                    FechaPago = pago.FechaPago,
                    MetodoPago = pago.MetodoPago,
                    Estado = pago.Estado
                })
                .ToList();
        }

        return View(modelo);
    }

    private bool MembresiaPerteneceACliente(int membresiaId, int clienteId)
    {
        return _membresiaService.ConsultarMembresiasDeCliente(clienteId)
            .Any(membresia => membresia.Id == membresiaId);
    }

    private void CargarOpciones(PagoFormViewModel modelo)
    {
        modelo.Clientes = _clienteService.Consultar()
            .Select(cliente => new SelectListItem(cliente.Nombre, cliente.Id.ToString(), cliente.Id == modelo.ClienteId))
            .ToList();

        if (modelo.ClienteId <= 0)
        {
            modelo.Membresias = Array.Empty<SelectListItem>();
            return;
        }

        modelo.Membresias = _membresiaService.ConsultarMembresiasDeCliente(modelo.ClienteId)
            .Select(membresia => new SelectListItem(
                $"{membresia.Plan.Nombre} · {membresia.FechaInicio:dd/MM/yyyy} - {membresia.FechaFin:dd/MM/yyyy}",
                membresia.Id.ToString(),
                membresia.Id == modelo.MembresiaId))
            .ToList();
    }
}
