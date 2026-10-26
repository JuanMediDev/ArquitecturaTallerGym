using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Gimnasio.Datos;

public class GimnasioDbContextFactory : IDesignTimeDbContextFactory<GimnasioDbContext>
{
    public GimnasioDbContext CreateDbContext(string[] args)
    {
        var directorioActual = new DirectoryInfo(Directory.GetCurrentDirectory());
        string? rutaConfiguracion = null;
        while (directorioActual is not null)
        {
            var rutaCandidata = Path.Combine(directorioActual.FullName, "Gimnasio.Presentacion", "appsettings.json");
            if (File.Exists(rutaCandidata))
            {
                rutaConfiguracion = rutaCandidata;
                break;
            }

            directorioActual = directorioActual.Parent;
        }

        if (rutaConfiguracion is null)
        {
            throw new FileNotFoundException("No se encontró appsettings.json de Presentación.");
        }

        using var documento = JsonDocument.Parse(File.ReadAllText(rutaConfiguracion));
        var cadenaConexion = documento.RootElement
            .GetProperty("ConnectionStrings")
            .GetProperty("GimnasioDb")
            .GetString()
            ?? throw new InvalidOperationException("No se encontró la cadena de conexión GimnasioDb.");

        var opciones = new DbContextOptionsBuilder<GimnasioDbContext>()
            .UseSqlServer(cadenaConexion)
            .Options;

        return new GimnasioDbContext(opciones);
    }
}
