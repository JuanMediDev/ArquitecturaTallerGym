# Pulso Gimnasio

Aplicación web académica para apoyar la administración diaria de un gimnasio. Permite gestionar clientes, membresías, pagos, clases grupales, reservas, asistencia y reportes desde una interfaz web en español.

## Descripción

El proyecto aplica arquitectura en capas y el patrón MVC para separar la interfaz, las reglas del negocio y la persistencia de datos. Está desarrollado con .NET 8, ASP.NET Core MVC, Entity Framework Core y SQLite. La solución contiene cuatro proyectos: Presentación, Negocio, Datos y Pruebas.

## Funcionalidades

- **Clientes:** consultar, registrar, editar, ver el perfil y cambiar el estado de un cliente.
- **Membresías:** administrar planes, asignar planes a clientes y consultar sus membresías e historial.
- **Pagos:** registrar pagos y consultar el historial por cliente. Los pagos se conservan; la interfaz no permite editarlos ni eliminarlos.
- **Clases:** programar sesiones grupales y consultar su instructor, horario, cupo y estado.
- **Reservas y asistencia:** reservar o cancelar un cupo, consultar reservas por clase y registrar asistencia.
- **Reportes:** consultar ingresos por periodo, membresías activas y asistencia por rango de fechas.
- **Instructores de demostración:** se incluyen datos iniciales para poder programar clases y explorar los flujos de reservas.

Las reglas de negocio se ejecutan en la capa Negocio. Entre otras validaciones, se comprueba el acceso del cliente antes de reservar, la vigencia y el pago de su membresía, el cupo disponible y la existencia de una reserva activa antes de registrar asistencia.

## Objetivo académico

El objetivo es aplicar los conceptos de arquitectura de software vistos en el taller mediante una solución pequeña y funcional. En particular, se busca:

- Separar las responsabilidades en Presentación, Negocio, Acceso a Datos y Base de Datos.
- Organizar la interfaz web con el patrón MVC.
- Mantener las reglas en servicios de Negocio y el acceso a datos detrás de interfaces de repositorio.
- Usar inyección de dependencias, Entity Framework Core y migraciones para persistir información.
- Verificar reglas de negocio mediante pruebas automatizadas con xUnit y Moq.

## Estructura de la solución

```text
Gimnasio.sln
├── Gimnasio.Presentacion  ASP.NET Core MVC, controladores, vistas y ViewModels
├── Gimnasio.Negocio       Entidades, servicios, reglas e interfaces de repositorio
├── Gimnasio.Datos         DbContext, repositorios EF Core y migraciones
└── Gimnasio.Pruebas       Pruebas automatizadas de la capa de Negocio
```

La Presentación invoca servicios de Negocio; Negocio define las interfaces de persistencia; Datos implementa esas interfaces. La Presentación referencia Datos únicamente para registrar las implementaciones en el contenedor de dependencias.

## Requisitos para ejecutar

- **Visual Studio 2022 o posterior**, con la carga de trabajo **ASP.NET y desarrollo web**, o un editor equivalente.
- **SDK de .NET 8**.

No se necesita instalar un servidor de base de datos ni `dotnet-ef`. Entity Framework Core usa SQLite y crea/aplica las migraciones al iniciar la aplicación. El archivo se guarda en `Gimnasio.Presentacion/App_Data/gimnasio.db`; esa carpeta y sus archivos auxiliares están excluidos de Git para que cada desarrollador tenga sus propios datos locales.

### Preparar y ejecutar

Clona o actualiza el repositorio y ejecuta la aplicación desde Visual Studio estableciendo `Gimnasio.Presentacion` como proyecto de inicio, o desde la carpeta raíz de la solución con:

```powershell
dotnet run --project Gimnasio.Presentacion
```

En el primer inicio se crea el archivo SQLite, se aplica el esquema y se cargan los instructores de demostración. No se copian los datos que existían en SQL Server LocalDB; cada instalación empieza con una base nueva.

Para ejecutar las pruebas automatizadas:

```powershell
dotnet test Gimnasio.sln
```

## Alcance

Es un proyecto académico enfocado en la gestión básica de un gimnasio. No incluye inicio de sesión ni roles, pasarelas de pago reales, notificaciones, API pública, administración avanzada de varias sedes ni configuración de despliegue.
