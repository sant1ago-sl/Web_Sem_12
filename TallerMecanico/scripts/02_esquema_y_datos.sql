-- Esquema y datos de ejemplo. Se ejecuta sobre la base ya seleccionada (local o nube).
-- Es re-ejecutable: borra y recrea las tablas.
IF OBJECT_ID(N'dbo.Reparaciones', N'U') IS NOT NULL DROP TABLE dbo.Reparaciones;
IF OBJECT_ID(N'dbo.Vehiculos', N'U') IS NOT NULL DROP TABLE dbo.Vehiculos;
GO

CREATE TABLE dbo.Vehiculos (
    Id          INT IDENTITY(1,1) CONSTRAINT PK_Vehiculos PRIMARY KEY,
    Placa       NVARCHAR(10)  NOT NULL CONSTRAINT UQ_Vehiculos_Placa UNIQUE,
    Marca       NVARCHAR(50)  NOT NULL,
    Modelo      NVARCHAR(50)  NOT NULL,
    Anio        INT           NOT NULL CONSTRAINT CK_Vehiculos_Anio CHECK (Anio BETWEEN 1950 AND 2100),
    Propietario NVARCHAR(100) NOT NULL,
    Telefono    NVARCHAR(20)  NULL
);

CREATE TABLE dbo.Reparaciones (
    Id          INT IDENTITY(1,1) CONSTRAINT PK_Reparaciones PRIMARY KEY,
    VehiculoId  INT           NOT NULL,
    Descripcion NVARCHAR(200) NOT NULL,
    Costo       DECIMAL(10,2) NOT NULL CONSTRAINT CK_Reparaciones_Costo CHECK (Costo >= 0),
    Fecha       DATE          NOT NULL CONSTRAINT DF_Reparaciones_Fecha DEFAULT (CONVERT(date, GETDATE())),
    Estado      NVARCHAR(20)  NOT NULL CONSTRAINT DF_Reparaciones_Estado DEFAULT (N'Pendiente'),
    CONSTRAINT FK_Reparaciones_Vehiculos FOREIGN KEY (VehiculoId) REFERENCES dbo.Vehiculos(Id),
    CONSTRAINT CK_Reparaciones_Estado CHECK (Estado IN (N'Pendiente', N'EnProceso', N'Finalizada'))
);
GO

INSERT INTO dbo.Vehiculos (Placa, Marca, Modelo, Anio, Propietario, Telefono) VALUES
 (N'ABC-123', N'Toyota',  N'Corolla', 2018, N'Carlos Ramirez', N'987654321'),
 (N'DEF-456', N'Hyundai', N'Accent',  2020, N'Lucia Fernandez', N'912345678'),
 (N'GHI-789', N'Kia',     N'Rio',     2016, N'Jorge Mendoza',  NULL),
 (N'JKL-012', N'Nissan',  N'Sentra',  2022, N'Ana Torres',     N'955443322');

INSERT INTO dbo.Reparaciones (VehiculoId, Descripcion, Costo, Fecha, Estado) VALUES
 (1, N'Cambio de aceite y filtro',        120.00, '2026-10-01', N'Finalizada'),
 (1, N'Cambio de pastillas de freno',     250.50, '2026-10-20', N'EnProceso'),
 (2, N'Alineamiento y balanceo',           90.00, '2026-10-15', N'Finalizada'),
 (3, N'Reparacion de caja de cambios',   1800.00, '2026-11-02', N'Pendiente'),
 (4, N'Revision general de 10,000 km',    150.00, '2026-11-03', N'Pendiente');
GO
