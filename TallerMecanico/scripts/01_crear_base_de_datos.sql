-- Solo para entorno local (LocalDB). En la nube la base ya la crea el proveedor:
-- alli se ejecuta unicamente 02_esquema_y_datos.sql sobre la base existente.
IF DB_ID(N'TallerMecanicoDB') IS NULL
    CREATE DATABASE TallerMecanicoDB;
GO
