using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gimnasio.Presentacion.Models;

public class AsignarMembresiaViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Selecciona un cliente.")]
    [Display(Name = "Cliente")]
    public int ClienteId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecciona un plan.")]
    [Display(Name = "Plan")]
    public int PlanId { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de inicio")]
    public DateTime FechaInicio { get; set; } = DateTime.Today;

    public IReadOnlyList<SelectListItem> Clientes { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Planes { get; set; } = Array.Empty<SelectListItem>();
}
