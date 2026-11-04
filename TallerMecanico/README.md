# Taller Mecanico - Web API REST (ASP.NET Core .NET 10 + EF Core)

Entidades: `Vehiculo` (1) -> (N) `Reparacion`.
Base local: SQL Server (LocalDB). Base en la nube: PostgreSQL de Render (ver "Conclusiones").

## Estructura (separada para poder desplegar)
- `api/`      proyecto Web API (se despliega con el `Dockerfile` de la raiz)
  - `api/wwwroot/index.html` panel de pruebas; se abre en la raiz `/` al iniciar la API
- `scripts/`
  - `01_crear_base_de_datos.sql` (solo local)
  - `02_esquema_y_datos.sql` (SQL Server)
  - `02_esquema_y_datos_postgres.sql` (PostgreSQL, para Render)
- `pruebas.http` pruebas de todos los codigos (200, 201, 204, 400, 404, 409)

## Local (SQL Server LocalDB)
1. Ejecutar los scripts 01 y 02 en `(localdb)\MSSQLLocalDB`.
2. Abrir `TallerMecanico.slnx` en Visual Studio y ejecutar (F5): abre el panel de pruebas; Swagger esta en `/swagger`.

## Nube (Render: API + PostgreSQL)
1. Render -> New -> PostgreSQL (plan Free). Copiar la **External Database URL** para cargar el script
   y la **Internal Database URL** para la API.
2. Cargar el esquema: `psql "<External Database URL>" -f scripts/02_esquema_y_datos_postgres.sql`
3. Render -> New -> Web Service (runtime Docker, plan Free) con variables de entorno:
   - `DatabaseProvider` = `Postgres`
   - `ConnectionStrings__TallerDb` = Internal Database URL (se acepta el formato `postgresql://...`)
   - `Cors__AllowedOrigin` = `https://<tu-app>.onrender.com`
   - `PORT` = `8080`
4. Abrir `https://<tu-app>.onrender.com/` (panel de pruebas) y ejecutar todas las pruebas.

## Endpoints
- `GET/POST api/vehiculos`, `GET/PUT/DELETE api/vehiculos/{id}`, `GET api/vehiculos/{id}/reparaciones`
- `GET/POST api/reparaciones`, `GET/PUT/DELETE api/reparaciones/{id}`
- Filtros: `vehiculos?marca=&propietario=&anioDesde=&anioHasta=`, `reparaciones?estado=&vehiculoId=&costoMin=&costoMax=`
- 409: placa repetida, eliminar vehiculo con reparaciones, modificar reparacion finalizada, eliminar reparacion en proceso.

## Conclusiones (cambio de proveedor)
La base gratuita de Render es PostgreSQL, no SQL Server. Se agrego el paquete
`Npgsql.EntityFrameworkCore.PostgreSQL` y `UseNpgsql(...)` (se elige con `DatabaseProvider`), y el script
se adapto a PostgreSQL (`GENERATED ALWAYS AS IDENTITY`, `NUMERIC`, `CURRENT_DATE`, identificadores entre comillas).
El modelo, los controladores y las Data Annotations no cambian.
