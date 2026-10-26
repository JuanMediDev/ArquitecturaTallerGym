using Gimnasio.Negocio.Entidades;

namespace Gimnasio.Presentacion.Models;

public class ClienteListaViewModel
{
    public IReadOnlyList<ClienteFilaViewModel> Clientes { get; set; } = Array.Empty<ClienteFilaViewModel>();
}

public class ClienteFilaViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public EstadoCliente Estado { get; set; }
}
