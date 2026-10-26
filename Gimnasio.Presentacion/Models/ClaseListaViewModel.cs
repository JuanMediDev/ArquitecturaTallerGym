using Gimnasio.Negocio.Entidades;

namespace Gimnasio.Presentacion.Models;

public class ClaseListaViewModel
{
    public IReadOnlyList<ClaseFilaViewModel> Clases { get; set; } = Array.Empty<ClaseFilaViewModel>();
    public IReadOnlyList<InstructorFilaViewModel> Instructores { get; set; } = Array.Empty<InstructorFilaViewModel>();
}

public class InstructorFilaViewModel
{
    public string Nombre { get; set; } = string.Empty;
    public string Especialidad { get; set; } = string.Empty;
}

public class ClaseFilaViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public string Especialidad { get; set; } = string.Empty;
    public DateTime FechaHoraInicio { get; set; }
    public DateTime FechaHoraFin { get; set; }
    public int CupoMaximo { get; set; }
    public EstadoClase Estado { get; set; }
}
