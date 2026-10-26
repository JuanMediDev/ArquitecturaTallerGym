using Gimnasio.Negocio.Entidades;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gimnasio.Presentacion.Models;

public class PagoHistorialViewModel
{
    public int? ClienteId { get; set; }
    public string? NombreCliente { get; set; }
    public IReadOnlyList<SelectListItem> Clientes { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<PagoFilaViewModel> Pagos { get; set; } = Array.Empty<PagoFilaViewModel>();
}

public class PagoFilaViewModel
{
    public int MembresiaId { get; set; }
    public decimal Monto { get; set; }
    public DateTime FechaPago { get; set; }
    public string MetodoPago { get; set; } = string.Empty;
    public EstadoPago Estado { get; set; }
}
