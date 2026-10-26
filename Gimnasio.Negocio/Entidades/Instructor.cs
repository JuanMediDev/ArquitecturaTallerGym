namespace Gimnasio.Negocio.Entidades;

public class Instructor
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Especialidad { get; set; } = string.Empty;
    public ICollection<Clase> Clases { get; set; } = new List<Clase>();
}
