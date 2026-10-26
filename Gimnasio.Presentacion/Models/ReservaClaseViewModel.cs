using Gimnasio.Negocio.Entidades;

namespace Gimnasio.Presentacion.Models;

public class ReservaClaseViewModel
{
    public int ClaseId { get; set; }
    public string NombreClase { get; set; } = string.Empty;
    public string NombreInstructor { get; set; } = string.Empty;
    public DateTime FechaHoraInicio { get; set; }
    public DateTime FechaHoraFin { get; set; }
    public int CupoMaximo { get; set; }
    public int ReservasActivas { get; set; }
    public IReadOnlyList<ReservaFilaViewModel> Reservas { get; set; } = Array.Empty<ReservaFilaViewModel>();
}

public class ReservaFilaViewModel
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public DateTime FechaReserva { get; set; }
    public EstadoReserva Estado { get; set; }
    public bool TieneAsistencia { get; set; }
}
