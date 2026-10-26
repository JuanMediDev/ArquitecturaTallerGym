namespace Gimnasio.Negocio.Entidades;

public class Clase
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int InstructorId { get; set; }
    public DateTime FechaHoraInicio { get; set; }
    public DateTime FechaHoraFin { get; set; }
    public int CupoMaximo { get; set; }
    public EstadoClase Estado { get; set; }
    public Instructor Instructor { get; set; } = null!;
    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    public ICollection<Asistencia> Asistencias { get; set; } = new List<Asistencia>();
}
