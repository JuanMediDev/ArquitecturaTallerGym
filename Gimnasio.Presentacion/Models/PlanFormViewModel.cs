using System.ComponentModel.DataAnnotations;

namespace Gimnasio.Presentacion.Models;

public class PlanFormViewModel
{
    [Required(ErrorMessage = "El nombre del plan es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar 100 caracteres.")]
    [Display(Name = "Nombre del plan")]
    public string Nombre { get; set; } = string.Empty;

    [Range(1, 3650, ErrorMessage = "La duración debe ser de al menos un día.")]
    [Display(Name = "Duración (días)")]
    public int DuracionDias { get; set; }

    [Range(0.01, 999999999, ErrorMessage = "El precio debe ser mayor que cero.")]
    [Display(Name = "Precio")]
    public decimal Precio { get; set; }

    [Required(ErrorMessage = "Describe los beneficios del plan.")]
    [StringLength(500, ErrorMessage = "Los beneficios no pueden superar 500 caracteres.")]
    [Display(Name = "Beneficios")]
    public string Beneficios { get; set; } = string.Empty;
}
