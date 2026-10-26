using Gimnasio.Negocio.Entidades;

namespace Gimnasio.Negocio.Interfaces;

public interface IAsistenciaRepositorio
{
    void Agregar(Asistencia asistencia);
    IReadOnlyList<Asistencia> ObtenerPorCliente(int clienteId);
    IReadOnlyList<Asistencia> ObtenerPorClase(int claseId);
    IReadOnlyList<Asistencia> ObtenerPorRango(DateTime fechaInicio, DateTime fechaFin);
}
