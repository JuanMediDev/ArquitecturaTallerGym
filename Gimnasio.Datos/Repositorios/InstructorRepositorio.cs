using Gimnasio.Negocio.Entidades;
using Gimnasio.Negocio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Datos.Repositorios;

public class InstructorRepositorio : IInstructorRepositorio
{
    private readonly GimnasioDbContext _contexto;

    public InstructorRepositorio(GimnasioDbContext contexto) => _contexto = contexto;

    public Instructor? ObtenerPorId(int id) => _contexto.Instructores
        .AsNoTracking()
        .FirstOrDefault(instructor => instructor.Id == id);

    public IReadOnlyList<Instructor> ObtenerTodos() => _contexto.Instructores
        .AsNoTracking()
        .OrderBy(instructor => instructor.Nombre)
        .ToList();
}
