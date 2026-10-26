namespace Gimnasio.Negocio.Entidades;

public class PlanMembresia
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int DuracionDias { get; set; }
    public decimal Precio { get; set; }
    public string Beneficios { get; set; } = string.Empty;
    public ICollection<Membresia> Membresias { get; set; } = new List<Membresia>();
}
