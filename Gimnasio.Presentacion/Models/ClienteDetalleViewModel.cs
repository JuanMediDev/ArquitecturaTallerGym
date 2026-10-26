using Gimnasio.Negocio.Entidades;

namespace Gimnasio.Presentacion.Models;

public class ClienteDetalleViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public EstadoCliente Estado { get; set; }
    public IReadOnlyList<MembresiaDetalleViewModel> Membresias { get; set; } = Array.Empty<MembresiaDetalleViewModel>();
}

public class MembresiaDetalleViewModel
{
    public string Plan { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public EstadoMembresia Estado { get; set; }
    public IReadOnlyList<PagoDetalleViewModel> Pagos { get; set; } = Array.Empty<PagoDetalleViewModel>();
}

public class PagoDetalleViewModel
{
    public decimal Monto { get; set; }
    public DateTime FechaPago { get; set; }
    public string MetodoPago { get; set; } = string.Empty;
    public EstadoPago Estado { get; set; }
}
