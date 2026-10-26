using Gimnasio.Negocio.Entidades;
using Gimnasio.Negocio.Servicios;
using Gimnasio.Presentacion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gimnasio.Presentacion.Controllers;

public class MembresiasController : Controller
{
    private readonly MembresiaService _membresiaService;
    private readonly ClienteService _clienteService;

    public MembresiasController(MembresiaService membresiaService, ClienteService clienteService)
    {
        _membresiaService = membresiaService;
        _clienteService = clienteService;
    }

    public IActionResult Planes()
    {
        var planes = _membresiaService.ListarPlanes()
            .Select(plan => new PlanFormViewModel
            {
                Nombre = plan.Nombre,
                DuracionDias = plan.DuracionDias,
                Precio = plan.Precio,
                Beneficios = plan.Beneficios
            })
            .ToList();
        return View(planes);
    }

    [HttpGet]
    public IActionResult CrearPlan() => View(new PlanFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CrearPlan(PlanFormViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        _membresiaService.CrearPlan(new PlanMembresia
        {
            Nombre = modelo.Nombre.Trim(),
            DuracionDias = modelo.DuracionDias,
            Precio = modelo.Precio,
            Beneficios = modelo.Beneficios.Trim()
        });

        TempData["Mensaje"] = "El plan se creó correctamente.";
        return RedirectToAction(nameof(Planes));
    }

    [HttpGet]
    public IActionResult Asignar(int? clienteId)
    {
        var modelo = CrearModeloAsignacion();
        if (clienteId.HasValue)
        {
            modelo.ClienteId = clienteId.Value;
        }

        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Asignar(AsignarMembresiaViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            CargarOpciones(modelo);
            return View(modelo);
        }

        var membresia = _membresiaService.AsignarPlanACliente(
            modelo.ClienteId,
            modelo.PlanId,
            modelo.FechaInicio);

        TempData["Mensaje"] = "La membresía se asignó correctamente.";
        return RedirectToAction(nameof(PorCliente), new { clienteId = membresia.ClienteId });
    }

    public IActionResult PorCliente(int clienteId)
    {
        var cliente = _clienteService.Consultar(clienteId);
        var membresias = _membresiaService.ConsultarMembresiasDeCliente(clienteId);
        var modelo = new MembresiaViewModel
        {
            ClienteId = cliente.Id,
            NombreCliente = cliente.Nombre,
            Membresias = membresias.Select(membresia => new MembresiaFilaViewModel
            {
                NombrePlan = membresia.Plan.Nombre,
                FechaInicio = membresia.FechaInicio,
                FechaFin = membresia.FechaFin,
                Estado = membresia.Estado,
                PrecioPlan = membresia.Plan.Precio,
                PagosRegistrados = membresia.Pagos.Count(pago => pago.Estado == EstadoPago.Registrado)
            }).ToList()
        };

        return View(modelo);
    }

    private AsignarMembresiaViewModel CrearModeloAsignacion()
    {
        var modelo = new AsignarMembresiaViewModel();
        CargarOpciones(modelo);
        return modelo;
    }

    private void CargarOpciones(AsignarMembresiaViewModel modelo)
    {
        modelo.Clientes = _clienteService.Consultar()
            .Select(cliente => new SelectListItem(cliente.Nombre, cliente.Id.ToString(), cliente.Id == modelo.ClienteId))
            .ToList();
        modelo.Planes = _membresiaService.ListarPlanes()
            .Select(plan => new SelectListItem($"{plan.Nombre} · {plan.DuracionDias} días · {plan.Precio:C}", plan.Id.ToString(), plan.Id == modelo.PlanId))
            .ToList();
    }
}
