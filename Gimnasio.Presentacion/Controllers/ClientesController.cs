using Gimnasio.Negocio.Entidades;
using Gimnasio.Negocio.Servicios;
using Gimnasio.Presentacion.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gimnasio.Presentacion.Controllers;

public class ClientesController : Controller
{
    private readonly ClienteService _clienteService;

    public ClientesController(ClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    public IActionResult Index()
    {
        var modelo = new ClienteListaViewModel
        {
            Clientes = _clienteService.Consultar()
                .Select(cliente => new ClienteFilaViewModel
                {
                    Id = cliente.Id,
                    Nombre = cliente.Nombre,
                    Documento = cliente.Documento,
                    Correo = cliente.Correo,
                    Estado = cliente.Estado
                })
                .ToList()
        };

        return View(modelo);
    }

    public IActionResult Detalle(int id)
    {
        var cliente = _clienteService.Consultar(id);
        var modelo = new ClienteDetalleViewModel
        {
            Id = cliente.Id,
            Nombre = cliente.Nombre,
            Documento = cliente.Documento,
            Correo = cliente.Correo,
            Telefono = cliente.Telefono,
            Estado = cliente.Estado,
            Membresias = cliente.Membresias
                .OrderByDescending(membresia => membresia.FechaInicio)
                .Select(membresia => new MembresiaDetalleViewModel
                {
                    Plan = membresia.Plan.Nombre,
                    FechaInicio = membresia.FechaInicio,
                    FechaFin = membresia.FechaFin,
                    Estado = membresia.Estado,
                    Pagos = membresia.Pagos
                        .OrderByDescending(pago => pago.FechaPago)
                        .Select(pago => new PagoDetalleViewModel
                        {
                            Monto = pago.Monto,
                            FechaPago = pago.FechaPago,
                            MetodoPago = pago.MetodoPago,
                            Estado = pago.Estado
                        })
                        .ToList()
                })
                .ToList()
        };

        return View(modelo);
    }

    [HttpGet]
    public IActionResult Crear() => View(new ClienteFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(ClienteFormViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        _clienteService.Registrar(MapearCliente(modelo));
        TempData["Mensaje"] = "Cliente registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Editar(int id)
    {
        var cliente = _clienteService.Consultar(id);
        return View(new ClienteFormViewModel
        {
            Id = cliente.Id,
            Nombre = cliente.Nombre,
            Documento = cliente.Documento,
            Correo = cliente.Correo,
            Telefono = cliente.Telefono,
            Estado = cliente.Estado
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Editar(int id, ClienteFormViewModel modelo)
    {
        if (id != modelo.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        _clienteService.Actualizar(MapearCliente(modelo));
        TempData["Mensaje"] = "Los datos del cliente se actualizaron correctamente.";
        return RedirectToAction(nameof(Detalle), new { id = modelo.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CambiarEstado(int id, EstadoCliente estado)
    {
        _clienteService.CambiarEstado(id, estado);
        TempData["Mensaje"] = "El estado del cliente se actualizó correctamente.";
        return RedirectToAction(nameof(Detalle), new { id });
    }

    private static Cliente MapearCliente(ClienteFormViewModel modelo) => new()
    {
        Id = modelo.Id,
        Nombre = modelo.Nombre.Trim(),
        Documento = modelo.Documento.Trim(),
        Correo = modelo.Correo.Trim(),
        Telefono = modelo.Telefono.Trim(),
        Estado = modelo.Estado
    };
}
