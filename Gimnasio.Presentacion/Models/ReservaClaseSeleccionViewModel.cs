using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gimnasio.Presentacion.Models;

public class ReservaClaseSeleccionViewModel
{
    public IReadOnlyList<SelectListItem> Clases { get; set; } = Array.Empty<SelectListItem>();
}
