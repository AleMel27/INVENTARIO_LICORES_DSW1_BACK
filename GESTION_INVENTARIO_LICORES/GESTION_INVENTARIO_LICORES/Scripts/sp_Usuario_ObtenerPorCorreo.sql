CREATE OR ALTER PROCEDURE sp_Usuario_ObtenerPorCorreo
    @Correo VARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        U.IdUsuario,
        U.Nombres,
        U.Apellidos,
        U.Correo,
        U.PasswordHash,
        U.IdRol,
        R.Nombre AS NombreRol,
        U.Estado
    FROM Usuarios U
    INNER JOIN Roles R
        ON R.IdRol = U.IdRol
    WHERE U.Correo = @Correo;
END;
GO
