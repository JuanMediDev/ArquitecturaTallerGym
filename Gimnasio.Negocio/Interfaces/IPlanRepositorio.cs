using Gimnasio.Negocio.Entidades;

namespace Gimnasio.Negocio.Interfaces;

public interface IPlanRepositorio
{
    void Agregar(PlanMembresia plan);
    PlanMembresia? ObtenerPorId(int id);
    IReadOnlyList<PlanMembresia> ObtenerTodos();
}
