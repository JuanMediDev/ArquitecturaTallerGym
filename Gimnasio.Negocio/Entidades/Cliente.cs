namespace Gimnasio.Negocio.Entidades;

public class Cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public EstadoCliente Estado { get; set; }
    public ICollection<Membresia> Membresias { get; set; } = new List<Membresia>();
    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    public ICollection<Asistencia> Asistencias { get; set; } = new List<Asistencia>();
}
