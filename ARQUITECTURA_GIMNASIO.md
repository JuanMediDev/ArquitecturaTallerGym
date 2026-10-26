# Taller N-Capas y MVC — Plataforma Web para Gimnasio

> **Instrucciones para el agente (leer primero)**
> - Proyecto **universitario**. Implementar **SOLO** lo descrito aquí. No agregar funcionalidades, patrones ni librerías extra.
> - Arquitectura **simple**: nada de microservicios, CQRS, MediatR, AutoMapper, Identity/JWT, Docker, caché ni eventos.
> - Trabajar **por pasos** (sección 9). Al terminar cada paso: compilar, correr pruebas y esperar confirmación antes del siguiente.
> - Código y nombres en español (entidades, servicios, vistas). Mantener métodos cortos y comentarios mínimos.

---

## 1. Objetivo y alcance

Aplicación web para administrar un gimnasio: **clientes, membresías, pagos, clases grupales, reservas, asistencia y reportes**.
Se aplican dos principios de separación de responsabilidades:
- **Arquitectura en 4 capas** (Presentación, Negocio, Acceso a Datos, Base de Datos).
- **Patrón MVC** dentro de la capa de presentación.

**Tecnología:** .NET 8, ASP.NET Core MVC, Entity Framework Core con SQLite, xUnit para pruebas.

**Fuera de alcance:** login/roles, pasarelas de pago reales, notificaciones, API pública, multi-sede avanzada, despliegue.

---

## 2. Estructura de la solución (3 proyectos + BD)

```
Gimnasio.sln
├── Gimnasio.Presentacion   (ASP.NET Core MVC)        → Vistas, Controladores, ViewModels
├── Gimnasio.Negocio        (Class Library)           → Entidades, Servicios (casos de uso), reglas, interfaces de repositorio
├── Gimnasio.Datos          (Class Library)           → DbContext, Repositorios (EF Core), Migraciones
└── Gimnasio.Pruebas        (xUnit)                   → Pruebas de reglas de negocio
```

| Capa (Clase 18) | Proyecto | Contenido |
|---|---|---|
| Presentación | `Gimnasio.Presentacion` | Vistas Razor, Controladores, ViewModels, recepción de solicitudes |
| Negocio | `Gimnasio.Negocio` | Entidades de dominio, servicios/casos de uso, validaciones y reglas |
| Acceso a datos (Persistencia) | `Gimnasio.Datos` | DbContext, repositorios, consultas, comunicación con la BD |
| Base de datos | SQLite | Archivo local con tablas y estructuras persistentes |

### Dependencias entre proyectos (capas cerradas)

```
Presentacion  →  Negocio  ←  Datos
```

- `Presentacion` usa **solo servicios** de `Negocio`. Nunca usa `DbContext` ni repositorios.
- `Negocio` define las **interfaces** de repositorio (`IClienteRepositorio`, etc.); no conoce EF Core.
- `Datos` **implementa** esas interfaces (referencia a `Negocio`).
- La inyección de dependencias se registra en `Program.cs` de `Presentacion` (única referencia permitida a `Datos`, solo para registrar servicios).

> Regla de oro (Clase 19): **Controladores delgados, Modelos ricos.** El controlador solo coordina; las reglas viven en Negocio. La Vista no tiene lógica de negocio ni acceso a datos.

---

## 3. Modelo del dominio (`Gimnasio.Negocio/Entidades`)

Mantener entidades simples (POCO con propiedades de navegación básicas).

| Entidad | Propiedades principales |
|---|---|
| `Cliente` | Id, Nombre, Documento, Correo, Telefono, Estado (`Activo`/`Inactivo`) |
| `PlanMembresia` | Id, Nombre, DuracionDias, Precio, Beneficios |
| `Membresia` | Id, ClienteId, PlanId, FechaInicio, FechaFin, Estado (`Activa`/`Vencida`/`Cancelada`) |
| `Pago` | Id, MembresiaId, Monto, FechaPago, MetodoPago, Estado (`Registrado`/`Anulado`) |
| `Instructor` | Id, Nombre, Especialidad |
| `Clase` | Id, Nombre, InstructorId, FechaHoraInicio, FechaHoraFin, CupoMaximo, Estado (`Programada`/`Cancelada`/`Finalizada`) |
| `Reserva` | Id, ClienteId, ClaseId, FechaReserva, Estado (`Activa`/`Cancelada`) |
| `Asistencia` | Id, ClienteId, ClaseId, FechaHoraRegistro |

> Simplificación: **cada `Clase` representa una sesión programada** (fecha y hora concretas). No se crea entidad de sesión aparte.

**Relaciones:** Cliente 1—N Membresia · PlanMembresia 1—N Membresia · Membresia 1—N Pago · Instructor 1—N Clase · Cliente 1—N Reserva · Clase 1—N Reserva · Cliente 1—N Asistencia · Clase 1—N Asistencia.

---

## 4. Reglas de negocio → dónde se implementan

Todas viven en **servicios de `Gimnasio.Negocio`**, nunca en controladores ni vistas.

| # | Regla | Implementación |
|---|---|---|
| 1 | Cliente con historial de membresías y pagos | Relaciones 1—N; consultas de historial en `MembresiaService` / `PagoService` |
| 2 | Membresía requiere plan, fecha inicio, fecha fin y estado | Validación en `MembresiaService.Asignar` (FechaFin = FechaInicio + DuracionDias del plan) |
| 3 | Solo reserva quien cumple condiciones de acceso | `ReservaService.Reservar` → `ClienteTieneAcceso`: cliente `Activo` **y** membresía `Activa` vigente (FechaFin ≥ hoy) **y** al día en pagos (al menos un pago `Registrado` asociado a esa membresía) |
| 4 | Clase no supera capacidad máxima | `ReservaService.Reservar`: contar reservas `Activas` de la clase; si ≥ `CupoMaximo` → rechazar |
| 5 | Sin reserva duplicada del cliente en la misma sesión | `ReservaService.Reservar`: si ya existe reserva `Activa` (ClienteId, ClaseId) → rechazar. Índice único en BD como respaldo |
| 6 | Asistencia solo para sesión programada y asociada al cliente | `ReservaService.RegistrarAsistencia`: clase en estado `Programada`, cliente con reserva `Activa` en esa clase |
| 7 | Pagos conservan historial, no se modifican arbitrariamente | `IPagoRepositorio` **solo** expone `Agregar` y consultas. **Sin** Editar ni Eliminar. Corrección = anular (cambio controlado de estado a `Anulado`) o nuevo registro |

Los servicios lanzan `ReglaNegocioException(mensaje)`; el controlador la captura y la muestra en la vista.

---

## 5. Capa de Negocio (`Gimnasio.Negocio`)

```
Entidades/        Cliente, PlanMembresia, Membresia, Pago, Instructor, Clase, Reserva, Asistencia
Excepciones/      ReglaNegocioException
Interfaces/       IClienteRepositorio, IPlanRepositorio, IMembresiaRepositorio, IPagoRepositorio,
                  IInstructorRepositorio, IClaseRepositorio, IReservaRepositorio, IAsistenciaRepositorio
Servicios/        ClienteService, MembresiaService, PagoService, ClaseService, ReservaService, ReporteService
```

| Servicio | Casos de uso (solo estos) |
|---|---|
| `ClienteService` | Registrar, Consultar (lista/detalle), Actualizar, CambiarEstado |
| `MembresiaService` | CrearPlan, ListarPlanes, AsignarPlanACliente, ConsultarMembresiasDeCliente (vencimiento y estado), ActualizarEstadosVencidos |
| `PagoService` | RegistrarPago, HistorialDeCliente, ClienteAlDia |
| `ClaseService` | Programar (con instructor, horario, cupo), Listar programación, ListarInstructores |
| `ReservaService` | Reservar, Cancelar, ConsultarReservasDeClase, RegistrarAsistencia, ConsultarAsistencia |
| `ReporteService` | IngresosPorRango, MembresiasActivas, AsistenciaPorRango |

---

## 6. Capa de Acceso a Datos (`Gimnasio.Datos`)

```
GimnasioDbContext.cs     DbSets de las 8 entidades + configuración con Fluent API
Repositorios/            Una implementación por cada interfaz de Negocio (EF Core)
Migraciones/             Migración inicial
```

- Repositorios solo contienen consultas y persistencia (sin reglas de negocio).
- Configurar en `OnModelCreating`: claves, longitudes, enums como string e **índice único** en `Reserva(ClienteId, ClaseId)` para reservas activas (si es complejo, un índice único simple es suficiente).
- Cadena de conexión SQLite en `appsettings.json` de `Presentacion`; el archivo local se crea al iniciar la aplicación.

---

## 7. Capa de Presentación — MVC (`Gimnasio.Presentacion`)

### 7.1 Controladores (solo coordinan: reciben, validan formato, llaman al servicio, devuelven vista)

| Controlador | Acciones | RF |
|---|---|---|
| `ClientesController` | Index, Detalle, Crear, Editar, CambiarEstado | RF01 |
| `MembresiasController` | Planes, CrearPlan, Asignar, PorCliente | RF02 |
| `PagosController` | Registrar, Historial (por cliente) | RF03 |
| `ClasesController` | Index (programación), Programar | RF04 |
| `ReservasController` | Reservar, Cancelar, PorClase, RegistrarAsistencia | RF05 |
| `ReportesController` | Ingresos, MembresiasActivas, Asistencia | Reportes |

### 7.2 Vistas (`Views/<Controlador>/`)

| Módulo | Vistas |
|---|---|
| Clientes | `Index`, `Detalle`, `Crear`, `Editar` |
| Membresías | `Planes`, `CrearPlan`, `Asignar`, `PorCliente` |
| Pagos | `Registrar`, `Historial` |
| Clases | `Index`, `Programar` |
| Reservas | `Reservar`, `PorClase` (incluye cancelar y registrar asistencia) |
| Reportes | `Ingresos`, `MembresiasActivas`, `Asistencia` |
| Compartidas | `_Layout` con menú simple |

Las vistas solo muestran datos del ViewModel y envían formularios. **Sin lógica de negocio ni acceso a BD.**

### 7.3 Modelos

- **Modelos de dominio:** entidades de Negocio (sección 3).
- **ViewModels auxiliares** (`Models/`): `ClienteFormViewModel`, `ClienteListaViewModel`, `PlanFormViewModel`, `AsignarMembresiaViewModel`, `MembresiaViewModel`, `PagoFormViewModel`, `PagoHistorialViewModel`, `ClaseFormViewModel`, `ClaseListaViewModel`, `ReservaFormViewModel`, `ReservaClaseViewModel`, `ReporteViewModel`. Usar DataAnnotations (`[Required]`, `[Range]`) para validar formato de entrada.
- Mapeo entidad ↔ ViewModel **manual** (sin AutoMapper).

### 7.4 Flujo de una solicitud (ejemplo: reservar una clase)

```
Usuario ──(1) POST /Reservas/Reservar ──► ReservasController
ReservasController ──(2) valida ModelState (formato) ──► ReservaService.Reservar(clienteId, claseId)
ReservaService ──(3) aplica reglas 3,4,5 ──► repositorios (IClienteRepositorio, IMembresiaRepositorio,
                                              IPagoRepositorio, IReservaRepositorio, IClaseRepositorio)
Repositorios (Datos) ──(4) consultas EF Core ──► SQLite
ReservaService ──(5) guarda la Reserva ──► devuelve OK o lanza ReglaNegocioException
ReservasController ──(6) arma ViewModel con resultado o mensaje de error ──► Vista
Vista ──(7) renderiza HTML ──► Usuario
```

**Quién inicia y qué responsabilidad tiene cada componente:** Usuario inicia · Controlador coordina · Servicio (Modelo/Negocio) decide · Repositorio persiste · Vista presenta.

---

## 8. Cuestionario (respuestas para el documento del taller)

1. **¿Cómo se distribuyen las responsabilidades?**
   - *Capas:* Presentación (UI y solicitudes) → Negocio (casos de uso y reglas) → Datos (consultas y persistencia) → BD (almacenamiento físico).
   - *MVC:* Vista presenta y captura acciones · Controlador recibe, valida formato, invoca el servicio y elige la respuesta · Modelo contiene entidades y reglas.
2. **¿Dónde ubicar la regla que impide reservar sin cupos?** En la **capa de Negocio**, en `ReservaService.Reservar` (el Modelo en MVC). No en el controlador (evita *Fat Controller*) ni en la vista (evita *Smart View*).
3. **¿Qué componente consulta los pagos registrados?** El `PagoService` (Negocio) lo solicita al `PagoRepositorio` (Datos); el `PagosController` invoca al servicio y la vista `Historial` los muestra. El controlador y la vista nunca consultan la BD.
4. **3 cambios de interfaz sin tocar reglas de negocio:** (a) cambiar colores/estilos del `_Layout` y botones; (b) reordenar columnas o agregar filtros visuales en la tabla de clientes; (c) cambiar el formulario de reserva de lista desplegable a tarjetas o cambiar el idioma/formato de fechas mostrado.
5. **¿Es posible usar MVC dentro de una arquitectura en capas?** **Sí.** Son niveles de organización distintos: las capas separan responsabilidades técnicas de todo el sistema; MVC organiza la interacción web dentro de la capa de presentación. Aquí Vista y Controlador residen en Presentación, y el Modelo se apoya en Negocio y Datos, sin romper el aislamiento entre capas.

---

## 9. Plan de implementación por pasos (para el agente)

Paquetes: EF Core SQLite + Tools en `Datos`; Moq en `Pruebas`.
✔ *Criterio:* la solución compila.

**Paso 2 — Entidades y excepción.** Crear en `Negocio` las 8 entidades, enums de estado y `ReglaNegocioException`.
✔ *Criterio:* compila.

**Paso 3 — Interfaces de repositorio.** Crear las 8 interfaces en `Negocio/Interfaces` (Pago: solo Agregar, Anular y consultas).
✔ *Criterio:* compila.

**Paso 4 — Acceso a datos.** `GimnasioDbContext`, configuración Fluent API, 8 repositorios y migración inicial SQLite; crear la base local.
✔ *Criterio:* migración aplicada correctamente.

**Paso 5 — Servicios de negocio.** Implementar `ClienteService`, `MembresiaService`, `PagoService`, `ClaseService`, `ReservaService`, `ReporteService` con las reglas de la sección 4.
✔ *Criterio:* compila.

**Paso 6 — Pruebas de negocio (xUnit + Moq).** Una prueba por regla, usando repositorios simulados:
- R2: asignar membresía calcula FechaFin y estado correcto.
- R3: reservar falla si cliente inactivo, sin membresía vigente o sin pago registrado.
- R4: reservar falla cuando la clase está en cupo máximo.
- R5: reservar falla si ya existe reserva activa del mismo cliente.
- R6: asistencia falla si la clase no está programada o el cliente no tiene reserva.
- R7: `IPagoRepositorio` no expone editar/eliminar; el pago anulado conserva su registro.
- Caso feliz: reserva exitosa.
✔ *Criterio:* todas las pruebas pasan.

**Paso 7 — Presentación base.** Configurar `Program.cs` (DI de repositorios y servicios, cadena de conexión), `_Layout` con menú y manejo de `ReglaNegocioException` en controladores.
✔ *Criterio:* la app inicia y muestra el menú.

**Paso 8 — Módulos MVC (uno por vez, en este orden):** Clientes → Membresías → Pagos → Clases → Reservas/Asistencia → Reportes. Para cada uno: ViewModels, Controlador, Vistas. Mostrar mensajes de error de reglas en pantalla. Incluir `[ValidateAntiForgeryToken]` en los POST.
✔ *Criterio:* el flujo CRUD de cada módulo funciona manualmente.

**Paso 9 — Verificación final.** Ejecutar todas las pruebas, recorrer el flujo completo (cliente → plan → membresía → pago → clase → reserva → asistencia → reportes) y confirmar que ningún controlador o vista accede al `DbContext` ni contiene reglas de negocio.
✔ *Criterio:* checklist cumplido; entregar breve resumen de lo implementado.

---

## 10. Checklist de cumplimiento (revisar al final)

- [ ] 3 proyectos separados + proyecto de pruebas, con las dependencias de la sección 2.
- [ ] Ningún controlador/vista usa `DbContext` ni repositorios directamente.
- [ ] Las 7 reglas de negocio están en servicios de Negocio y probadas.
- [ ] Pagos sin editar/eliminar.
- [ ] Solo existen los módulos RF01–RF05 + reportes de ingresos, membresías activas y asistencia.
- [ ] Sin librerías ni funcionalidades fuera de alcance.
