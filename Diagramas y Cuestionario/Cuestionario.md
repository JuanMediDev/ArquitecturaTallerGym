# Respuestas del cuestionario de arquitectura

## 1. Distribución de responsabilidades entre capas y MVC

- **Presentación (`Gimnasio.Presentacion`):** recibe las solicitudes, presenta formularios y resultados, valida el formato de entrada y gestiona los casos de uso.
- **Negocio (`Gimnasio.Negocio`):** contiene las entidades, servicios y reglas que determinan la validez del flujo de operaciones.
- **Acceso a datos (`Gimnasio.Datos`):** implementa los repositorios, ejecuta consultas y persiste cambios.
- **Base de datos (SQLite):** conserva la información de manera persistente.

En **MVC**, que se utiliza dentro de la Presentación, la **Vista** muestra información y recibe la interacción; el **Controlador** recibe la solicitud, valida el modelo de entrada y llama al servicio adecuado; el **Modelo** representa los datos y el comportamiento del dominio. En este sistema, las entidades y las reglas del Modelo se ubican principalmente en la capa de Negocio. Los ViewModels son modelos específicos de las vistas para transportar los datos de la interfaz.

## 2. ¿Qué componente debería decidir si se puede reservar una clase?

En la capa de Negocio. El servicio consulta el número de reservas activas de la clase y lo compara con `CupoMaximo`; si ya se alcanzó el límite, rechaza la operación con `ReglaNegocioException`. Así, la misma regla se aplica aunque la solicitud no provenga de una vista específica, y no queda duplicada en el controlador o en la interfaz.

El repositorio  consulta y guarda, pero no decide la regla; la vista puede mostrar los cupos como información, pero no es la autoridad que permita o prohíba la reserva.

## 3. ¿Quién consulta el historial de pagos de un cliente y cómo se obtiene la información?

El servicio de Negocio que necesita esa información debe solicitarla mediante la interfaz `IPagoRepositorio`, implementada en la capa de Datos por `PagoRepositorio`. Por ejemplo, `PagoService.HistorialDeCliente` consulta el historial y `ReservaService` usa `ExisteRegistradoEnMembresia` para verificar el requisito de pago al autorizar una reserva.

En el flujo de la pantalla de historial, `PagosController` invoca `PagoService.HistorialDeCliente` y prepara los datos para la vista. El controlador y la vista no consultan directamente SQLite ni `GimnasioDbContext`.

## 4. Cambios en la interfaz que no afectan las reglas de negocio

1. Cambiar colores, tipografía, estilos del menú o apariencia de botones en `_Layout` y los estilos de la aplicación.
2. Reordenar las columnas de una tabla o cambiar cómo se presentan los clientes (por ejemplo, tabla frente a tarjetas), manteniendo los mismos datos y operaciones.
3. Cambiar el formulario de selección de clase de una lista desplegable a tarjetas, o modificar el formato visual de las fechas, sin alterar la validación de membresía, pago, duplicados ni cupo.

Los cambios se enfocan principalmente en la presentación y la experiencia del usuario, sin afectar las reglas de negocio que determinan las condiciones de reserva, pago o asistencia a clases. Las reglas del sistema se mantienen en la capa de negocios en la parte interna.

## 5. Compatibilidad de MVC y arquitectura en capas

Son formas complementarias de organizar el sistema: las **capas** separan responsabilidades técnicas a nivel de aplicación, mientras que **MVC** estructura la interacción entre usuario, interfaz y modelo dentro de la capa de Presentación.

En este proyecto, las vistas y los controladores están en `Gimnasio.Presentacion`; los controladores coordinan los casos de uso a través de servicios de `Gimnasio.Negocio`. Las reglas pertenecen a Negocio y la persistencia se delega a repositorios implementados en `Gimnasio.Datos`. Por ello, se puede aplicar MVC sin que la Presentación acceda directamente a la base de datos y sin mezclar las reglas del negocio con la interfaz.
