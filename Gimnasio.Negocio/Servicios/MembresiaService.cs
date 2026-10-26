using Gimnasio.Negocio.Entidades;
using Gimnasio.Negocio.Excepciones;
using Gimnasio.Negocio.Interfaces;

namespace Gimnasio.Negocio.Servicios;

public class MembresiaService
{
    private readonly IClienteRepositorio _clienteRepositorio;
    private readonly IPlanRepositorio _planRepositorio;
    private readonly IMembresiaRepositorio _membresiaRepositorio;

    public MembresiaService(
        IClienteRepositorio clienteRepositorio,
        IPlanRepositorio planRepositorio,
        IMembresiaRepositorio membresiaRepositorio)
    {
        _clienteRepositorio = clienteRepositorio;
        _planRepositorio = planRepositorio;
        _membresiaRepositorio = membresiaRepositorio;
    }

    public void CrearPlan(PlanMembresia plan)
    {
        _planRepositorio.Agregar(plan);
    }

    public IReadOnlyList<PlanMembresia> ListarPlanes() => _planRepositorio.ObtenerTodos();

    public Membresia AsignarPlanACliente(int clienteId, int planId, DateTime fechaInicio)
    {
        var cliente = _clienteRepositorio.ObtenerPorId(clienteId)
            ?? throw new ReglaNegocioException($"No se encontró el cliente con identificador {clienteId}.");
        var plan = _planRepositorio.ObtenerPorId(planId)
            ?? throw new ReglaNegocioException($"No se encontró el plan con identificador {planId}.");

        if (plan.DuracionDias <= 0)
        {
            throw new ReglaNegocioException("La duración del plan debe ser mayor que cero.");
        }

        var membresia = new Membresia
        {
            ClienteId = cliente.Id,
            PlanId = plan.Id,
            FechaInicio = fechaInicio,
            FechaFin = fechaInicio.AddDays(plan.DuracionDias),
            Estado = EstadoMembresia.Activa
        };

        _membresiaRepositorio.Agregar(membresia);
        return membresia;
    }

    public IReadOnlyList<Membresia> ConsultarMembresiasDeCliente(int clienteId)
    {
        if (_clienteRepositorio.ObtenerPorId(clienteId) is null)
        {
            throw new ReglaNegocioException($"No se encontró el cliente con identificador {clienteId}.");
        }

        ActualizarEstadosVencidos();
        return _membresiaRepositorio.ObtenerPorCliente(clienteId);
    }

    public int ActualizarEstadosVencidos(DateTime? fechaReferencia = null)
    {
        var fecha = (fechaReferencia ?? DateTime.Today).Date;
        var vencidas = _membresiaRepositorio.ObtenerVencidas(fecha);

        foreach (var membresia in vencidas)
        {
            membresia.Estado = EstadoMembresia.Vencida;
            _membresiaRepositorio.Actualizar(membresia);
        }

        return vencidas.Count;
    }
}
