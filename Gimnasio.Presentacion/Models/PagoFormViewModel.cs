using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gimnasio.Presentacion.Models;

public class PagoFormViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Selecciona un cliente.")]
    [Display(Name = "Cliente")]
    public int ClienteId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecciona una membresía.")]
    [Display(Name = "Membresía")]
    public int MembresiaId { get; set; }

    [Range(0.01, 999999999, ErrorMessage = "El monto debe ser mayor que cero.")]
    [Display(Name = "Monto")]
    public decimal Monto { get; set; }

    [Required(ErrorMessage = "El método de pago es obligatorio.")]
    [StringLength(50, ErrorMessage = "El método no puede superar 50 caracteres.")]
    [Display(Name = "Método de pago")]
    public string MetodoPago { get; set; } = string.Empty;

    [DataType(DataType.DateTime)]
    [Display(Name = "Fecha de pago")]
    public DateTime? FechaPago { get; set; }

    public IReadOnlyList<SelectListItem> Clientes { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Membresias { get; set; } = Array.Empty<SelectListItem>();
}
