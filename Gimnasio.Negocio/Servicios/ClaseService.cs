using Gimnasio.Negocio.Entidades;
using Gimnasio.Negocio.Excepciones;
using Gimnasio.Negocio.Interfaces;

namespace Gimnasio.Negocio.Servicios;

public class ClaseService
{
    private readonly IClaseRepositorio _claseRepositorio;
    private readonly IInstructorRepositorio _instructorRepositorio;

    public ClaseService(IClaseRepositorio claseRepositorio, IInstructorRepositorio instructorRepositorio)
    {
        _claseRepositorio = claseRepositorio;
        _instructorRepositorio = instructorRepositorio;
    }

    public void Programar(Clase clase)
    {
        if (_instructorRepositorio.ObtenerPorId(clase.InstructorId) is null)
        {
            throw new ReglaNegocioException($"No se encontró el instructor con identificador {clase.InstructorId}.");
        }

        if (clase.FechaHoraFin <= clase.FechaHoraInicio)
        {
            throw new ReglaNegocioException("La fecha y hora de finalización debe ser posterior al inicio.");
        }

        if (clase.CupoMaximo <= 0)
        {
            throw new ReglaNegocioException("El cupo máximo debe ser mayor que cero.");
        }

        clase.Estado = EstadoClase.Programada;
        _claseRepositorio.Agregar(clase);
    }

    public IReadOnlyList<Clase> ListarProgramacion() => _claseRepositorio.ObtenerProgramacion();

    public IReadOnlyList<Instructor> ListarInstructores() => _instructorRepositorio.ObtenerTodos();
}
