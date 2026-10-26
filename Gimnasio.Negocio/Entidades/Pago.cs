namespace Gimnasio.Negocio.Entidades;

public class Pago
{
    public int Id { get; set; }
    public int MembresiaId { get; set; }
    public decimal Monto { get; set; }
    public DateTime FechaPago { get; set; }
    public string MetodoPago { get; set; } = string.Empty;
    public EstadoPago Estado { get; set; }
    public Membresia Membresia { get; set; } = null!;
}
