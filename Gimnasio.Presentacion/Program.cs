using Gimnasio.Datos;
using Gimnasio.Datos.Repositorios;
using Gimnasio.Negocio.Interfaces;
using Gimnasio.Negocio.Servicios;
using Gimnasio.Presentacion.Filtros;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var cadenaConexion = builder.Configuration.GetConnectionString("GimnasioDb")
    ?? throw new InvalidOperationException("No se encontró la cadena de conexión GimnasioDb.");
var opcionesConexion = new SqliteConnectionStringBuilder(cadenaConexion);
var rutaBaseDatos = Path.GetFullPath(opcionesConexion.DataSource, builder.Environment.ContentRootPath);
Directory.CreateDirectory(Path.GetDirectoryName(rutaBaseDatos)!);
opcionesConexion.DataSource = rutaBaseDatos;

builder.Services.AddScoped<ReglaNegocioExceptionFilter>();
builder.Services.AddControllersWithViews(opciones =>
    opciones.Filters.AddService<ReglaNegocioExceptionFilter>());
builder.Services.AddDbContext<GimnasioDbContext>(opciones =>
    opciones.UseSqlite(opcionesConexion.ToString()));

builder.Services.AddScoped<IClienteRepositorio, ClienteRepositorio>();
builder.Services.AddScoped<IPlanRepositorio, PlanRepositorio>();
builder.Services.AddScoped<IMembresiaRepositorio, MembresiaRepositorio>();
builder.Services.AddScoped<IPagoRepositorio, PagoRepositorio>();
builder.Services.AddScoped<IInstructorRepositorio, InstructorRepositorio>();
builder.Services.AddScoped<IClaseRepositorio, ClaseRepositorio>();
builder.Services.AddScoped<IReservaRepositorio, ReservaRepositorio>();
builder.Services.AddScoped<IAsistenciaRepositorio, AsistenciaRepositorio>();

builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<MembresiaService>();
builder.Services.AddScoped<PagoService>();
builder.Services.AddScoped<ClaseService>();
builder.Services.AddScoped<ReservaService>();
builder.Services.AddScoped<ReporteService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var contexto = scope.ServiceProvider.GetRequiredService<GimnasioDbContext>();
    contexto.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
