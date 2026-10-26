using Gimnasio.Negocio.Entidades;
using Gimnasio.Negocio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Datos.Repositorios;

public class ClaseRepositorio : IClaseRepositorio
{
    private readonly GimnasioDbContext _contexto;

    public ClaseRepositorio(GimnasioDbContext contexto) => _contexto = contexto;

    public void Agregar(Clase clase)
    {
        _contexto.Clases.Add(clase);
        _contexto.SaveChanges();
    }

    public Clase? ObtenerPorId(int id) => _contexto.Clases
        .AsNoTracking()
        .Include(clase => clase.Instructor)
        .FirstOrDefault(clase => clase.Id == id);

    public IReadOnlyList<Clase> ObtenerProgramacion() => _contexto.Clases
        .AsNoTracking()
        .Include(clase => clase.Instructor)
        .OrderBy(clase => clase.FechaHoraInicio)
        .ToList();
}
