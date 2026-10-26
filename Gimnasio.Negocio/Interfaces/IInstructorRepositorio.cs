using Gimnasio.Negocio.Entidades;

namespace Gimnasio.Negocio.Interfaces;

public interface IInstructorRepositorio
{
    Instructor? ObtenerPorId(int id);
    IReadOnlyList<Instructor> ObtenerTodos();
}
