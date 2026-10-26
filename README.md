# Pulso Gimnasio

Aplicación web académica para administrar operaciones básicas de un gimnasio: clientes, membresías, pagos, clases grupales, reservas, asistencia y reportes. Implementa arquitectura en capas y MVC con ASP.NET Core, .NET 8, Entity Framework Core y SQLite.

## Funcionalidades implementadas

- **Clientes:** listado, registro, edición, detalle y cambio de estado.
- **Membresías:** creación de planes, asignación a clientes y consulta de membresías.
- **Pagos:** registro e historial por cliente. El historial se conserva; los pagos se pueden anular, no editar ni eliminar.
- **Clases:** programación de sesiones con instructor, horario, cupo máximo y estado.
- **Reservas y asistencia:** reserva y cancelación de cupos, consulta por clase y registro de asistencia.
- **Reportes:** ingresos por periodo, membresías activas y asistencia por rango de fechas.
- **Instructores de demostración:** tres instructores iniciales permiten explorar la programación de clases. `Instructor` es una entidad persistida: la migración crea la tabla `Instructores`, inserta esos datos iniciales y relaciona cada clase mediante `InstructorId`.

Las reglas de negocio residen en los servicios de `Gimnasio.Negocio`. Por ejemplo, `ReservaService.Reservar` verifica cliente activo, membresía vigente con pago registrado, ausencia de una reserva duplicada y cupo disponible. Los rechazos esperados de reserva, cancelación y asistencia se muestran como alertas contextuales; las vistas presentan el mensaje, pero no determinan la regla.

## Arquitectura y solución

```text
Gimnasio.sln
├── Gimnasio.Presentacion  ASP.NET Core MVC: controladores, vistas Razor y ViewModels
├── Gimnasio.Negocio       Entidades, servicios, reglas e interfaces de repositorio
├── Gimnasio.Datos         DbContext, repositorios EF Core, migraciones y datos semilla
└── Gimnasio.Pruebas       Pruebas automatizadas de reglas de negocio (xUnit y Moq)
```

Los controladores de Presentación invocan servicios de Negocio. Negocio define las interfaces de repositorio y Datos las implementa con EF Core. `Gimnasio.Presentacion` registra estas implementaciones mediante inyección de dependencias. La base de datos local es SQLite.

### Persistencia

La migración inicial define las tablas `Clientes`, `PlanesMembresia`, `Membresias`, `Pagos`, `Instructores`, `Clases`, `Reservas` y `Asistencias`, junto con sus relaciones. En particular, `Clases.InstructorId` referencia `Instructores.Id`; los tres instructores de demostración se insertan como datos iniciales. `Reservas` tiene un índice único filtrado por cliente y clase para reservas activas.

Al iniciar la aplicación, `Program.cs` crea la carpeta de la base de datos si hace falta y aplica las migraciones pendientes. El archivo SQLite y los auxiliares locales no se requieren en Git: un clon nuevo crea su propia base en `Gimnasio.Presentacion/App_Data/gimnasio.db`.

## Ejecutar desde un clon nuevo

### Requisitos

- SDK de .NET 8 instalado.
- Para Visual Studio, la carga de trabajo **ASP.NET y desarrollo web**.
- No se requiere servidor de base de datos ni instalar `dotnet-ef`.

### Pasos

Desde la carpeta raíz del repositorio, ejecuta:

```powershell
dotnet run --project Gimnasio.Presentacion\Gimnasio.Presentacion.csproj
```

La primera ejecución restaura las dependencias de los proyectos, crea la carpeta `App_Data`, genera la base SQLite, aplica la migración y carga los instructores iniciales. Después, abre la URL local indicada por ASP.NET Core en la consola. En Visual Studio también se puede seleccionar `Gimnasio.Presentacion` como proyecto de inicio y ejecutar.

Para compilar toda la solución y ejecutar las pruebas:

```powershell
dotnet build Gimnasio.sln
dotnet test Gimnasio.sln
```

## Diagramas del entregable

En `Diagramas y Cuestionario` están las respuestas del cuestionario, los diagramas PNG y los diagramas editables `Diagrama_Capas_Final.drawio` y `Diagrama_MVC_Final.drawio`. Los diagramas de capas y MVC muestran `Instructor` como entidad de Negocio, su persistencia y su relación con `Clase`. El instructor no aparece en el flujo de reserva porque no participa en las decisiones de autorización ni disponibilidad.

## Alcance

Es un proyecto académico enfocado en la gestión básica de un gimnasio. No incluye autenticación ni roles, pasarelas de pago reales, notificaciones, API pública, administración avanzada de varias sedes ni despliegue.
