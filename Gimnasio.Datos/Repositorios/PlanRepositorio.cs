using Gimnasio.Negocio.Entidades;
using Gimnasio.Negocio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Datos.Repositorios;

public class PlanRepositorio : IPlanRepositorio
{
    private readonly GimnasioDbContext _contexto;

    public PlanRepositorio(GimnasioDbContext contexto) => _contexto = contexto;

    public void Agregar(PlanMembresia plan)
    {
        _contexto.PlanesMembresia.Add(plan);
        _contexto.SaveChanges();
    }

    public PlanMembresia? ObtenerPorId(int id) => _contexto.PlanesMembresia
        .AsNoTracking()
        .FirstOrDefault(plan => plan.Id == id);

    public IReadOnlyList<PlanMembresia> ObtenerTodos() => _contexto.PlanesMembresia
        .AsNoTracking()
        .OrderBy(plan => plan.Nombre)
        .ToList();
}
