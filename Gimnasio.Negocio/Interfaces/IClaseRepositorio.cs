using Gimnasio.Negocio.Entidades;

namespace Gimnasio.Negocio.Interfaces;

public interface IClaseRepositorio
{
    void Agregar(Clase clase);
    Clase? ObtenerPorId(int id);
    IReadOnlyList<Clase> ObtenerProgramacion();
}
