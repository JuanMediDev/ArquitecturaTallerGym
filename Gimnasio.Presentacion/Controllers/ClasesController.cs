using Gimnasio.Negocio.Entidades;
using Gimnasio.Negocio.Servicios;
using Gimnasio.Presentacion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gimnasio.Presentacion.Controllers;

public class ClasesController : Controller
{
    private readonly ClaseService _claseService;

    public ClasesController(ClaseService claseService)
    {
        _claseService = claseService;
    }

    public IActionResult Index()
    {
        var modelo = new ClaseListaViewModel
        {
            Instructores = _claseService.ListarInstructores()
                .Select(instructor => new InstructorFilaViewModel
                {
                    Nombre = instructor.Nombre,
                    Especialidad = instructor.Especialidad
                })
                .ToList(),
            Clases = _claseService.ListarProgramacion()
                .Select(clase => new ClaseFilaViewModel
                {
                    Id = clase.Id,
                    Nombre = clase.Nombre,
                    Instructor = clase.Instructor.Nombre,
                    Especialidad = clase.Instructor.Especialidad,
                    FechaHoraInicio = clase.FechaHoraInicio,
                    FechaHoraFin = clase.FechaHoraFin,
                    CupoMaximo = clase.CupoMaximo,
                    Estado = clase.Estado
                })
                .ToList()
        };

        return View(modelo);
    }

    [HttpGet]
    public IActionResult Programar()
    {
        var modelo = new ClaseFormViewModel();
        CargarInstructores(modelo);
        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Programar(ClaseFormViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            CargarInstructores(modelo);
            return View(modelo);
        }

        _claseService.Programar(new Clase
        {
            Nombre = modelo.Nombre.Trim(),
            InstructorId = modelo.InstructorId,
            FechaHoraInicio = modelo.FechaHoraInicio,
            FechaHoraFin = modelo.FechaHoraFin,
            CupoMaximo = modelo.CupoMaximo
        });

        TempData["Mensaje"] = "La clase se programó correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private void CargarInstructores(ClaseFormViewModel modelo)
    {
        modelo.Instructores = _claseService.ListarInstructores()
            .Select(instructor => new SelectListItem(
                $"{instructor.Nombre} · {instructor.Especialidad}",
                instructor.Id.ToString(),
                instructor.Id == modelo.InstructorId))
            .ToList();
    }
}
