using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gimnasio.Presentacion.Models;

public class ReservaFormViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Selecciona un cliente.")]
    [Display(Name = "Cliente")]
    public int ClienteId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecciona una clase.")]
    [Display(Name = "Clase")]
    public int ClaseId { get; set; }

    public IReadOnlyList<SelectListItem> Clientes { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Clases { get; set; } = Array.Empty<SelectListItem>();
}
