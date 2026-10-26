namespace Gimnasio.Negocio.Entidades;

public class Reserva
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public int ClaseId { get; set; }
    public DateTime FechaReserva { get; set; }
    public EstadoReserva Estado { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public Clase Clase { get; set; } = null!;
}
