# Taller Mecánico · Web API REST (ASP.NET Core .NET 10 + EF Core + LINQ)

Entidades: `Vehiculo` (1) → (N) `Reparacion`. Controladores con `DbContext` directo, Data Annotations, Swagger y CORS con origen concreto.
Incluye un panel web (`/`) que ejecuta todas las pruebas y muestra los códigos 200, 201, 204, 400, 404 y 409.

## Proveedores usados
| Servicio | Proveedor | Plan |
|---|---|---|
| Hosting de la API | Render (Web Service, Docker) | Free |
| Base de datos | Render PostgreSQL | Free |
| Base de datos local | SQL Server LocalDB | — |

**Enlace de la API (Swagger):** `https://<tu-app>.onrender.com/swagger`  ← reemplazar tras el despliegue

## Estructura
- `api/` proyecto Web API · `api/wwwroot/index.html` panel de pruebas
- `scripts/01_crear_base_de_datos.sql` y `scripts/02_esquema_y_datos.sql` (SQL Server, local)
- `scripts/02_esquema_y_datos_postgres.sql` (PostgreSQL, nube)
- `pruebas.http` · `Dockerfile` · `../render.yaml` (despliegue en Render)

## Ejecutar en local (SQL Server LocalDB)
1. Ejecutar en `(localdb)\MSSQLLocalDB` los scripts `01` y `02`.
2. Guardar la cadena de conexión en user-secrets (no se sube al repositorio):
   ```bash
   dotnet user-secrets set "ConnectionStrings:TallerDb" "Server=(localdb)\MSSQLLocalDB;Database=TallerMecanicoDB;Trusted_Connection=True;TrustServerCertificate=True;" --project TallerMecanico/api
   ```
3. Abrir `TallerMecanico.slnx` en Visual Studio y pulsar F5. Se abre el panel en `http://localhost:5062/`; Swagger en `/swagger`.

## Despliegue en la nube
El archivo `render.yaml` crea la base PostgreSQL y la API ya conectadas:
1. Render → **New → Blueprint** → elegir este repositorio → **Apply**.
2. En el primer arranque la API ejecuta `02_esquema_y_datos_postgres.sql` si las tablas no existen (`InitializeDatabase=true`).
3. Abrir `https://<tu-app>.onrender.com/` (panel) y `/swagger`.

La cadena de conexión la inyecta Render como variable de entorno (`ConnectionStrings__TallerDb`); no está en el repositorio.
Variables: `DatabaseProvider=Postgres`, `InitializeDatabase=true`, `PORT=8080`. CORS usa `RENDER_EXTERNAL_URL` (o `Cors__AllowedOrigin`).

## Endpoints
- `GET/POST api/vehiculos`, `GET/PUT/DELETE api/vehiculos/{id}`, `GET api/vehiculos/{id}/reparaciones`
- `GET/POST api/reparaciones`, `GET/PUT/DELETE api/reparaciones/{id}`
- Filtros: `vehiculos?marca=&propietario=&anioDesde=&anioHasta=` · `reparaciones?estado=&vehiculoId=&costoMin=&costoMax=`
- 409: placa repetida, eliminar vehículo con reparaciones, modificar reparación finalizada, eliminar reparación en proceso.

## Conclusiones
La base gratuita de Render es PostgreSQL, no SQL Server. Se agregó `Npgsql.EntityFrameworkCore.PostgreSQL` y `UseNpgsql(...)`
(se elige con `DatabaseProvider`) y se adaptó el script (`GENERATED ALWAYS AS IDENTITY`, `NUMERIC`, `CURRENT_DATE`,
identificadores entre comillas). El modelo, los controladores y las validaciones no cambian.
Limitaciones del plan gratuito: la API se duerme tras inactividad (el primer request tarda) y la base gratuita de Render caduca (confirmar el plazo vigente).
