namespace Gimnasio.Negocio.Entidades;

public class Asistencia
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public int ClaseId { get; set; }
    public DateTime FechaHoraRegistro { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public Clase Clase { get; set; } = null!;
}
