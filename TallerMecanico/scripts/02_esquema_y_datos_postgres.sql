-- Version PostgreSQL de 02_esquema_y_datos.sql (para la base gratuita de Render).
-- Los identificadores van entre comillas para conservar las mayusculas que usa EF Core.
-- Es re-ejecutable: borra y recrea las tablas.
DROP TABLE IF EXISTS "Reparaciones";
DROP TABLE IF EXISTS "Vehiculos";

CREATE TABLE "Vehiculos" (
    "Id"          INT GENERATED ALWAYS AS IDENTITY CONSTRAINT "PK_Vehiculos" PRIMARY KEY,
    "Placa"       VARCHAR(10)  NOT NULL CONSTRAINT "UQ_Vehiculos_Placa" UNIQUE,
    "Marca"       VARCHAR(50)  NOT NULL,
    "Modelo"      VARCHAR(50)  NOT NULL,
    "Anio"        INT          NOT NULL CONSTRAINT "CK_Vehiculos_Anio" CHECK ("Anio" BETWEEN 1950 AND 2100),
    "Propietario" VARCHAR(100) NOT NULL,
    "Telefono"    VARCHAR(20)  NULL
);

CREATE TABLE "Reparaciones" (
    "Id"          INT GENERATED ALWAYS AS IDENTITY CONSTRAINT "PK_Reparaciones" PRIMARY KEY,
    "VehiculoId"  INT           NOT NULL,
    "Descripcion" VARCHAR(200)  NOT NULL,
    "Costo"       NUMERIC(10,2) NOT NULL CONSTRAINT "CK_Reparaciones_Costo" CHECK ("Costo" >= 0),
    "Fecha"       DATE          NOT NULL DEFAULT CURRENT_DATE,
    "Estado"      VARCHAR(20)   NOT NULL DEFAULT 'Pendiente',
    CONSTRAINT "FK_Reparaciones_Vehiculos" FOREIGN KEY ("VehiculoId") REFERENCES "Vehiculos"("Id"),
    CONSTRAINT "CK_Reparaciones_Estado" CHECK ("Estado" IN ('Pendiente', 'EnProceso', 'Finalizada'))
);

INSERT INTO "Vehiculos" ("Placa", "Marca", "Modelo", "Anio", "Propietario", "Telefono") VALUES
 ('ABC-123', 'Toyota',  'Corolla', 2018, 'Carlos Ramirez',  '987654321'),
 ('DEF-456', 'Hyundai', 'Accent',  2020, 'Lucia Fernandez', '912345678'),
 ('GHI-789', 'Kia',     'Rio',     2016, 'Jorge Mendoza',   NULL),
 ('JKL-012', 'Nissan',  'Sentra',  2022, 'Ana Torres',      '955443322');

INSERT INTO "Reparaciones" ("VehiculoId", "Descripcion", "Costo", "Fecha", "Estado") VALUES
 (1, 'Cambio de aceite y filtro',       120.00, '2026-10-01', 'Finalizada'),
 (1, 'Cambio de pastillas de freno',    250.50, '2026-10-20', 'EnProceso'),
 (2, 'Alineamiento y balanceo',          90.00, '2026-10-15', 'Finalizada'),
 (3, 'Reparacion de caja de cambios',  1800.00, '2026-11-02', 'Pendiente'),
 (4, 'Revision general de 10,000 km',   150.00, '2026-11-03', 'Pendiente');
