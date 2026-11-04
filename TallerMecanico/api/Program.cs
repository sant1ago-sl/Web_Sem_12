using Microsoft.EntityFrameworkCore;
using TallerMecanico.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// La cadena de conexion NO va en el repositorio: se lee de la variable de entorno
// ConnectionStrings__TallerDb (nube) o de user-secrets / appsettings.Development.json (local).
var connectionString = builder.Configuration.GetConnectionString("TallerDb")
    ?? throw new InvalidOperationException("Falta la cadena de conexion 'TallerDb'.");

// Proveedor: "SqlServer" (local, LocalDB) o "Postgres" (base gratuita de Render).
// Se elige con la variable de entorno DatabaseProvider.
var provider = builder.Configuration["DatabaseProvider"] ?? "SqlServer";
builder.Services.AddDbContext<TallerContext>(o =>
{
    if (provider.Equals("Postgres", StringComparison.OrdinalIgnoreCase))
        o.UseNpgsql(PostgresConnection(connectionString));
    else
        o.UseSqlServer(connectionString);
});

// Render entrega la URL como postgresql://usuario:clave@host[:puerto]/base; Npgsql necesita pares clave=valor.
static string PostgresConnection(string cs)
{
    if (!cs.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) &&
        !cs.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
        return cs;

    var uri = new Uri(cs);
    var user = uri.UserInfo.Split(':', 2);
    return new Npgsql.NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.IsDefaultPort || uri.Port < 0 ? 5432 : uri.Port,
        Database = uri.AbsolutePath.TrimStart('/'),
        Username = Uri.UnescapeDataString(user[0]),
        Password = user.Length > 1 ? Uri.UnescapeDataString(user[1]) : null,
        SslMode = Npgsql.SslMode.Require
    }.ConnectionString;
}

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// CORS con un origen concreto (configurable por Cors__AllowedOrigin en la nube)
// Render define RENDER_EXTERNAL_URL (la URL publica del servicio) de forma automatica.
var origen = builder.Configuration["Cors:AllowedOrigin"]
    ?? Environment.GetEnvironmentVariable("RENDER_EXTERNAL_URL")
    ?? "http://localhost:5173";
builder.Services.AddCors(o => o.AddPolicy("Frontend", p =>
    p.WithOrigins(origen).AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// Primer despliegue en la nube: con InitializeDatabase=true se ejecuta el script SQL de PostgreSQL
// (scripts/02_esquema_y_datos_postgres.sql) solo si la tabla Vehiculos aun no existe.
if (provider.Equals("Postgres", StringComparison.OrdinalIgnoreCase) && builder.Configuration.GetValue<bool>("InitializeDatabase"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<TallerContext>();
    try
    {
        var existe = await db.Database
            .SqlQueryRaw<bool>("""SELECT to_regclass('public."Vehiculos"') IS NOT NULL AS "Value" """)
            .SingleAsync();
        if (!existe)
        {
            var script = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "scripts", "02_esquema_y_datos_postgres.sql"));
            await db.Database.ExecuteSqlRawAsync(script);
            app.Logger.LogInformation("Script SQL ejecutado: tablas y datos de ejemplo creados.");
        }
        else app.Logger.LogInformation("La base ya tiene las tablas; no se ejecuta el script.");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "No se pudo inicializar la base de datos.");
    }
}

app.UseDefaultFiles();
app.UseStaticFiles(); // sirve el panel de pruebas (wwwroot/index.html) en la raiz

app.MapOpenApi();
app.UseSwaggerUI(o =>
{
    o.SwaggerEndpoint("/openapi/v1.json", "Taller Mecanico API v1");
    o.RoutePrefix = "swagger";
});

// Si la plataforma termina HTTPS en su proxy (Render, Railway...), no redirigimos.
if (!string.Equals(Environment.GetEnvironmentVariable("DISABLE_HTTPS_REDIRECT"), "true", StringComparison.OrdinalIgnoreCase))
    app.UseHttpsRedirection();

app.UseCors("Frontend");
app.UseAuthorization();
app.MapControllers();

// Muestra en la consola los enlaces al panel de pruebas y a Swagger al iniciar.
app.Lifetime.ApplicationStarted.Register(() =>
{
    foreach (var url in app.Urls)
    {
        app.Logger.LogInformation("Panel de pruebas: {Url}/", url);
        app.Logger.LogInformation("Swagger:          {Url}/swagger", url);
    }
});

app.Run();

