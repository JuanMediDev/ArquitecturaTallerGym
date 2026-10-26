using Gimnasio.Negocio.Entidades;
using Gimnasio.Negocio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Datos.Repositorios;

public class AsistenciaRepositorio : IAsistenciaRepositorio
{
    private readonly GimnasioDbContext _contexto;

    public AsistenciaRepositorio(GimnasioDbContext contexto) => _contexto = contexto;

    public void Agregar(Asistencia asistencia)
    {
        _contexto.Asistencias.Add(asistencia);
        _contexto.SaveChanges();
    }

    public IReadOnlyList<Asistencia> ObtenerPorCliente(int clienteId) => _contexto.Asistencias
        .AsNoTracking()
        .Include(asistencia => asistencia.Clase)
        .Where(asistencia => asistencia.ClienteId == clienteId)
        .OrderByDescending(asistencia => asistencia.FechaHoraRegistro)
        .ToList();

    public IReadOnlyList<Asistencia> ObtenerPorClase(int claseId) => _contexto.Asistencias
        .AsNoTracking()
        .Include(asistencia => asistencia.Cliente)
        .Where(asistencia => asistencia.ClaseId == claseId)
        .OrderBy(asistencia => asistencia.FechaHoraRegistro)
        .ToList();

    public IReadOnlyList<Asistencia> ObtenerPorRango(DateTime fechaInicio, DateTime fechaFin) => _contexto.Asistencias
        .AsNoTracking()
        .Include(asistencia => asistencia.Cliente)
        .Include(asistencia => asistencia.Clase)
        .Where(asistencia => asistencia.FechaHoraRegistro >= fechaInicio
            && asistencia.FechaHoraRegistro <= fechaFin)
        .OrderBy(asistencia => asistencia.FechaHoraRegistro)
        .ToList();
}
