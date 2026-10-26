namespace Gimnasio.Negocio.Entidades;

public class Membresia
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public int PlanId { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public EstadoMembresia Estado { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public PlanMembresia Plan { get; set; } = null!;
    public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
