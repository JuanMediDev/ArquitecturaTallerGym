using Gimnasio.Negocio.Entidades;

namespace Gimnasio.Presentacion.Models;

public class MembresiaViewModel
{
    public int ClienteId { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public IReadOnlyList<MembresiaFilaViewModel> Membresias { get; set; } = Array.Empty<MembresiaFilaViewModel>();
}

public class MembresiaFilaViewModel
{
    public string NombrePlan { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public EstadoMembresia Estado { get; set; }
    public decimal PrecioPlan { get; set; }
    public int PagosRegistrados { get; set; }
}
