# Taller Mecánico · Web API REST

API REST con ASP.NET Core (.NET 10), Entity Framework Core y LINQ para gestionar vehículos y sus reparaciones.
Incluye un panel web que ejecuta todas las pruebas de los endpoints.

## Ejecutar el proyecto
1. Abrir `TallerMecanico/TallerMecanico.slnx` en Visual Studio y pulsar F5.
2. Se abre el panel de pruebas en `http://localhost:5062/`.
3. Swagger está en `http://localhost:5062/swagger`.

Los enlaces también aparecen en la consola al iniciar:

```
Panel de pruebas: http://localhost:5062/
Swagger:          http://localhost:5062/swagger
```

Desde terminal, en la raíz del repositorio:

```bash
dotnet run --project TallerMecanico/api --launch-profile http
```

## Modelo de datos
`Vehiculo` (1) → (N) `Reparacion`

## Estructura
- `TallerMecanico/api/` proyecto Web API
- `TallerMecanico/api/wwwroot/index.html` panel de pruebas
- `TallerMecanico/scripts/` scripts SQL (SQL Server y PostgreSQL)
- `TallerMecanico/pruebas.http` pruebas de los endpoints
- `TallerMecanico/Dockerfile` imagen para desplegar

## Base de datos local (SQL Server LocalDB)
Ejecutar en `(localdb)\MSSQLLocalDB`:
1. `scripts/01_crear_base_de_datos.sql`
2. `scripts/02_esquema_y_datos.sql`

## Endpoints
| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/vehiculos` | Lista con filtros `marca`, `propietario`, `anioDesde`, `anioHasta` |
| GET | `/api/vehiculos/{id}` | Obtiene un vehículo |
| GET | `/api/vehiculos/{id}/reparaciones` | Reparaciones de un vehículo |
| POST | `/api/vehiculos` | Crea un vehículo |
| PUT | `/api/vehiculos/{id}` | Actualiza un vehículo |
| DELETE | `/api/vehiculos/{id}` | Elimina un vehículo |
| GET | `/api/reparaciones` | Lista con filtros `estado`, `vehiculoId`, `costoMin`, `costoMax` |
| GET | `/api/reparaciones/{id}` | Obtiene una reparación |
| POST | `/api/reparaciones` | Crea una reparación |
| PUT | `/api/reparaciones/{id}` | Actualiza una reparación |
| DELETE | `/api/reparaciones/{id}` | Elimina una reparación |

## Códigos de respuesta
- **200** GET correcto · **201** POST correcto · **204** PUT y DELETE correctos
- **400** datos inválidos, id distinto del cuerpo o clave foránea inexistente
- **404** recurso inexistente
- **409** regla de negocio incumplida:
  - placa repetida
  - eliminar un vehículo con reparaciones
  - modificar una reparación finalizada
  - eliminar una reparación en proceso

## Despliegue en la nube (Render: API + PostgreSQL)
1. Crear una base PostgreSQL gratuita en Render y cargar `scripts/02_esquema_y_datos_postgres.sql`.
2. Crear un Web Service (Docker, plan Free) con las variables de entorno:
   - `DatabaseProvider` = `Postgres`
   - `ConnectionStrings__TallerDb` = URL interna de la base de Render
   - `Cors__AllowedOrigin` = `https://<tu-app>.onrender.com`
   - `PORT` = `8080`
3. Abrir `https://<tu-app>.onrender.com/` y ejecutar todas las pruebas.

La cadena de conexión no se sube al repositorio: se configura como variable de entorno.

## Conclusiones
La base gratuita de Render es PostgreSQL, por lo que se agregó el proveedor `Npgsql.EntityFrameworkCore.PostgreSQL`
(`UseNpgsql`) y se adaptó el script. El modelo, los controladores y las validaciones no cambian.
En local se usa SQL Server (LocalDB).
