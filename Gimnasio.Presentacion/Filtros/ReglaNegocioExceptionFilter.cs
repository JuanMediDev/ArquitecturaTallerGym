using Gimnasio.Negocio.Excepciones;
using Gimnasio.Presentacion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Gimnasio.Presentacion.Filtros;

public class ReglaNegocioExceptionFilter : IExceptionFilter
{
    private readonly IModelMetadataProvider _modelMetadataProvider;

    public ReglaNegocioExceptionFilter(IModelMetadataProvider modelMetadataProvider)
    {
        _modelMetadataProvider = modelMetadataProvider;
    }

    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not ReglaNegocioException excepcion)
        {
            return;
        }

        var modelo = new ErrorViewModel
        {
            Mensaje = excepcion.Message,
            RequestId = context.HttpContext.TraceIdentifier
        };

        context.Result = new ViewResult
        {
            ViewName = "Error",
            ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<ErrorViewModel>(
                _modelMetadataProvider,
                new ModelStateDictionary())
            {
                Model = modelo
            }
        };
        context.ExceptionHandled = true;
    }
}
