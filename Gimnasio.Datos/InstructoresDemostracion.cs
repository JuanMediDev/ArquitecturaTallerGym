using Gimnasio.Negocio.Entidades;

namespace Gimnasio.Datos;

public enum InstructorDemostracionId
{
    FuerzaAcondicionamiento = 1,
    CiclismoIndoor = 2,
    MovilidadEstiramiento = 3
}

public static class InstructoresDemostracion
{
    // Para probar flujos sin instructores en clases y reservas, elimina las entradas de este arreglo y crea/aplica una migración.
    public static readonly Instructor[] Datos =
    {
        new()
        {
            Id = (int)InstructorDemostracionId.FuerzaAcondicionamiento,
            Nombre = "Valeria Ríos",
            Especialidad = "Fuerza y acondicionamiento"
        },
        new()
        {
            Id = (int)InstructorDemostracionId.CiclismoIndoor,
            Nombre = "Mateo Cruz",
            Especialidad = "Ciclismo indoor"
        },
        new()
        {
            Id = (int)InstructorDemostracionId.MovilidadEstiramiento,
            Nombre = "Camila Torres",
            Especialidad = "Movilidad y estiramiento"
        }
    };
}
