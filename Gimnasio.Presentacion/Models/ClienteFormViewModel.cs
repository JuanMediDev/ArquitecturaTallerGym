using System.ComponentModel.DataAnnotations;
using Gimnasio.Negocio.Entidades;

namespace Gimnasio.Presentacion.Models;

public class ClienteFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(120, ErrorMessage = "El nombre no puede superar 120 caracteres.")]
    [Display(Name = "Nombre completo")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El documento es obligatorio.")]
    [StringLength(30, ErrorMessage = "El documento no puede superar 30 caracteres.")]
    [Display(Name = "Documento")]
    public string Documento { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingresa un correo válido.")]
    [StringLength(150, ErrorMessage = "El correo no puede superar 150 caracteres.")]
    [Display(Name = "Correo electrónico")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [Phone(ErrorMessage = "Ingresa un teléfono válido.")]
    [StringLength(30, ErrorMessage = "El teléfono no puede superar 30 caracteres.")]
    [Display(Name = "Teléfono")]
    public string Telefono { get; set; } = string.Empty;

    [Display(Name = "Estado")]
    public EstadoCliente Estado { get; set; } = EstadoCliente.Activo;
}
