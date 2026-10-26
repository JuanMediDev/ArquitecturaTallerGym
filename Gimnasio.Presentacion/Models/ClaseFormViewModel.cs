using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gimnasio.Presentacion.Models;

public class ClaseFormViewModel
{
    [Required(ErrorMessage = "El nombre de la clase es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar 100 caracteres.")]
    [Display(Name = "Nombre de la clase")]
    public string Nombre { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Selecciona un instructor.")]
    [Display(Name = "Instructor")]
    public int InstructorId { get; set; }

    [Required(ErrorMessage = "La fecha y hora de inicio son obligatorias.")]
    [DataType(DataType.DateTime)]
    [Display(Name = "Inicio")]
    public DateTime FechaHoraInicio { get; set; }

    [Required(ErrorMessage = "La fecha y hora de fin son obligatorias.")]
    [DataType(DataType.DateTime)]
    [Display(Name = "Fin")]
    public DateTime FechaHoraFin { get; set; }

    [Range(1, 500, ErrorMessage = "El cupo debe ser de al menos una persona.")]
    [Display(Name = "Cupo máximo")]
    public int CupoMaximo { get; set; }

    public IReadOnlyList<SelectListItem> Instructores { get; set; } = Array.Empty<SelectListItem>();
}
