using Gimnasio.Negocio.Entidades;
using Gimnasio.Negocio.Excepciones;
using Gimnasio.Negocio.Interfaces;
using Gimnasio.Negocio.Servicios;
using Moq;

namespace Gimnasio.Pruebas;

public class ReglasNegocioTests
{
    private const int ClienteId = 10;
    private const int ClaseId = 20;
    private const int MembresiaId = 30;

    [Fact]
    public void AsignarPlanACliente_CalculaFechaFinYEstadoActiva()
    {
        var clienteRepositorio = new Mock<IClienteRepositorio>();
        var planRepositorio = new Mock<IPlanRepositorio>();
        var membresiaRepositorio = new Mock<IMembresiaRepositorio>();
        var fechaInicio = new DateTime(2026, 1, 10);
        var cliente = new Cliente { Id = ClienteId };
        var plan = new PlanMembresia { Id = 5, DuracionDias = 30 };
        clienteRepositorio.Setup(repositorio => repositorio.ObtenerPorId(ClienteId)).Returns(cliente);
        planRepositorio.Setup(repositorio => repositorio.ObtenerPorId(plan.Id)).Returns(plan);

        var servicio = new MembresiaService(clienteRepositorio.Object, planRepositorio.Object, membresiaRepositorio.Object);
        var membresia = servicio.AsignarPlanACliente(ClienteId, plan.Id, fechaInicio);

        Assert.Equal(ClienteId, membresia.ClienteId);
        Assert.Equal(plan.Id, membresia.PlanId);
        Assert.Equal(fechaInicio, membresia.FechaInicio);
        Assert.Equal(fechaInicio.AddDays(30), membresia.FechaFin);
        Assert.Equal(EstadoMembresia.Activa, membresia.Estado);
        membresiaRepositorio.Verify(repositorio => repositorio.Agregar(membresia), Times.Once);
    }

    [Fact]
    public void Reservar_RechazaClienteInactivo()
    {
        var cliente = new Cliente { Id = ClienteId, Estado = EstadoCliente.Inactivo };
        var (servicio, reservaRepositorio, _, membresiaRepositorio, pagoRepositorio) = CrearServicioReserva(cliente);

        var excepcion = Assert.Throws<ReglaNegocioException>(() => servicio.Reservar(ClienteId, ClaseId));

        Assert.Contains("clientes activos", excepcion.Message);
        reservaRepositorio.Verify(repositorio => repositorio.Agregar(It.IsAny<Reserva>()), Times.Never);
        membresiaRepositorio.Verify(repositorio => repositorio.ObtenerPorCliente(It.IsAny<int>()), Times.Never);
        pagoRepositorio.Verify(repositorio => repositorio.ExisteRegistradoEnMembresia(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void Reservar_RechazaClienteSinMembresiaVigente()
    {
        var (servicio, reservaRepositorio, _, _, _) = CrearServicioReserva(membresias: Array.Empty<Membresia>());

        var excepcion = Assert.Throws<ReglaNegocioException>(() => servicio.Reservar(ClienteId, ClaseId));

        Assert.Contains("membresía activa vigente", excepcion.Message);
        reservaRepositorio.Verify(repositorio => repositorio.Agregar(It.IsAny<Reserva>()), Times.Never);
    }

    [Fact]
    public void Reservar_RechazaMembresiaSinPagoRegistrado()
    {
        var membresia = CrearMembresiaVigente();
        var (servicio, reservaRepositorio, _, _, _) = CrearServicioReserva(
            membresias: new[] { membresia },
            pagoRegistrado: false);

        var excepcion = Assert.Throws<ReglaNegocioException>(() => servicio.Reservar(ClienteId, ClaseId));

        Assert.Contains("pagos registrados", excepcion.Message);
        reservaRepositorio.Verify(repositorio => repositorio.Agregar(It.IsAny<Reserva>()), Times.Never);
    }

    [Fact]
    public void Reservar_RechazaClaseSinCupoDisponible()
    {
        var (servicio, reservaRepositorio, _, _, _) = CrearServicioReserva(cupoMaximo: 1, reservasActivas: 1);

        var excepcion = Assert.Throws<ReglaNegocioException>(() => servicio.Reservar(ClienteId, ClaseId));

        Assert.Contains("cupo máximo", excepcion.Message);
        reservaRepositorio.Verify(repositorio => repositorio.Agregar(It.IsAny<Reserva>()), Times.Never);
    }

    [Fact]
    public void Reservar_RechazaReservaActivaDuplicada()
    {
        var (servicio, reservaRepositorio, _, _, _) = CrearServicioReserva(reservaDuplicada: true);

        var excepcion = Assert.Throws<ReglaNegocioException>(() => servicio.Reservar(ClienteId, ClaseId));

        Assert.Contains("reserva activa", excepcion.Message);
        reservaRepositorio.Verify(repositorio => repositorio.Agregar(It.IsAny<Reserva>()), Times.Never);
    }

    [Fact]
    public void RegistrarAsistencia_RechazaClaseNoProgramada()
    {
        var claseRepositorio = new Mock<IClaseRepositorio>();
        var reservaRepositorio = new Mock<IReservaRepositorio>();
        var asistenciaRepositorio = new Mock<IAsistenciaRepositorio>();
        claseRepositorio.Setup(repositorio => repositorio.ObtenerPorId(ClaseId))
            .Returns(new Clase { Id = ClaseId, Estado = EstadoClase.Cancelada });
        var servicio = CrearServicioAsistencia(claseRepositorio, reservaRepositorio, asistenciaRepositorio);

        var excepcion = Assert.Throws<ReglaNegocioException>(() => servicio.RegistrarAsistencia(ClienteId, ClaseId));

        Assert.Contains("clase programada", excepcion.Message);
        reservaRepositorio.Verify(repositorio => repositorio.ExisteActiva(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        asistenciaRepositorio.Verify(repositorio => repositorio.Agregar(It.IsAny<Asistencia>()), Times.Never);
    }

    [Fact]
    public void RegistrarAsistencia_RechazaClienteSinReservaActiva()
    {
        var claseRepositorio = new Mock<IClaseRepositorio>();
        var reservaRepositorio = new Mock<IReservaRepositorio>();
        var asistenciaRepositorio = new Mock<IAsistenciaRepositorio>();
        claseRepositorio.Setup(repositorio => repositorio.ObtenerPorId(ClaseId))
            .Returns(new Clase { Id = ClaseId, Estado = EstadoClase.Programada });
        reservaRepositorio.Setup(repositorio => repositorio.ExisteActiva(ClienteId, ClaseId)).Returns(false);
        var servicio = CrearServicioAsistencia(claseRepositorio, reservaRepositorio, asistenciaRepositorio);

        var excepcion = Assert.Throws<ReglaNegocioException>(() => servicio.RegistrarAsistencia(ClienteId, ClaseId));

        Assert.Contains("reserva activa", excepcion.Message);
        asistenciaRepositorio.Verify(repositorio => repositorio.Agregar(It.IsAny<Asistencia>()), Times.Never);
    }

    [Fact]
    public void PagoRepositorio_NoExponeEdicionNiEliminacionYServicioSolicitaAnulacion()
    {
        var nombresMetodos = typeof(IPagoRepositorio).GetMethods().Select(metodo => metodo.Name).ToArray();
        Assert.DoesNotContain("Editar", nombresMetodos);
        Assert.DoesNotContain("Actualizar", nombresMetodos);
        Assert.DoesNotContain("Eliminar", nombresMetodos);
        Assert.DoesNotContain("Borrar", nombresMetodos);

        const int pagoId = 7;
        var pagoRepositorio = new Mock<IPagoRepositorio>();
        var servicio = new PagoService(Mock.Of<IMembresiaRepositorio>(), pagoRepositorio.Object);

        servicio.AnularPago(pagoId);

        pagoRepositorio.Verify(repositorio => repositorio.Anular(pagoId), Times.Once);
    }

    [Fact]
    public void Reservar_ConAccesoYCupoDisponible_CreaReservaActiva()
    {
        var (servicio, reservaRepositorio, _, _, _) = CrearServicioReserva();
        Reserva? reservaGuardada = null;
        reservaRepositorio.Setup(repositorio => repositorio.Agregar(It.IsAny<Reserva>()))
            .Callback<Reserva>(reserva => reservaGuardada = reserva);

        var reserva = servicio.Reservar(ClienteId, ClaseId);

        Assert.Equal(ClienteId, reserva.ClienteId);
        Assert.Equal(ClaseId, reserva.ClaseId);
        Assert.Equal(EstadoReserva.Activa, reserva.Estado);
        Assert.InRange(reserva.FechaReserva, DateTime.Now.AddMinutes(-1), DateTime.Now.AddMinutes(1));
        Assert.Same(reserva, reservaGuardada);
        reservaRepositorio.Verify(repositorio => repositorio.Agregar(It.IsAny<Reserva>()), Times.Once);
    }

    private static (ReservaService Servicio,
        Mock<IReservaRepositorio> ReservaRepositorio,
        Mock<IClaseRepositorio> ClaseRepositorio,
        Mock<IMembresiaRepositorio> MembresiaRepositorio,
        Mock<IPagoRepositorio> PagoRepositorio) CrearServicioReserva(
            Cliente? cliente = null,
            IReadOnlyList<Membresia>? membresias = null,
            bool pagoRegistrado = true,
            int cupoMaximo = 10,
            int reservasActivas = 0,
            bool reservaDuplicada = false)
    {
        var clienteRepositorio = new Mock<IClienteRepositorio>();
        var membresiaRepositorio = new Mock<IMembresiaRepositorio>();
        var pagoRepositorio = new Mock<IPagoRepositorio>();
        var claseRepositorio = new Mock<IClaseRepositorio>();
        var reservaRepositorio = new Mock<IReservaRepositorio>();
        var asistenciaRepositorio = new Mock<IAsistenciaRepositorio>();

        clienteRepositorio.Setup(repositorio => repositorio.ObtenerPorId(ClienteId))
            .Returns(cliente ?? new Cliente { Id = ClienteId, Estado = EstadoCliente.Activo });
        membresiaRepositorio.Setup(repositorio => repositorio.ObtenerPorCliente(ClienteId))
            .Returns(membresias ?? new[] { CrearMembresiaVigente() });
        pagoRepositorio.Setup(repositorio => repositorio.ExisteRegistradoEnMembresia(MembresiaId))
            .Returns(pagoRegistrado);
        claseRepositorio.Setup(repositorio => repositorio.ObtenerPorId(ClaseId))
            .Returns(new Clase { Id = ClaseId, CupoMaximo = cupoMaximo, Estado = EstadoClase.Programada });
        reservaRepositorio.Setup(repositorio => repositorio.ExisteActiva(ClienteId, ClaseId))
            .Returns(reservaDuplicada);
        reservaRepositorio.Setup(repositorio => repositorio.ContarActivasPorClase(ClaseId))
            .Returns(reservasActivas);

        var servicio = new ReservaService(
            clienteRepositorio.Object,
            membresiaRepositorio.Object,
            pagoRepositorio.Object,
            claseRepositorio.Object,
            reservaRepositorio.Object,
            asistenciaRepositorio.Object);

        return (servicio, reservaRepositorio, claseRepositorio, membresiaRepositorio, pagoRepositorio);
    }

    private static ReservaService CrearServicioAsistencia(
        Mock<IClaseRepositorio> claseRepositorio,
        Mock<IReservaRepositorio> reservaRepositorio,
        Mock<IAsistenciaRepositorio> asistenciaRepositorio)
    {
        return new ReservaService(
            Mock.Of<IClienteRepositorio>(),
            Mock.Of<IMembresiaRepositorio>(),
            Mock.Of<IPagoRepositorio>(),
            claseRepositorio.Object,
            reservaRepositorio.Object,
            asistenciaRepositorio.Object);
    }

    private static Membresia CrearMembresiaVigente() => new()
    {
        Id = MembresiaId,
        ClienteId = ClienteId,
        FechaFin = DateTime.Today.AddDays(10),
        Estado = EstadoMembresia.Activa
    };
}
