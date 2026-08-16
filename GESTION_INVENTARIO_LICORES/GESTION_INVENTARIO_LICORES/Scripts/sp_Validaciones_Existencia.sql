CREATE OR ALTER PROCEDURE sp_Almacen_ExisteNombre
    @Nombre VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CAST(
        CASE
            WHEN EXISTS (
                SELECT 1
                FROM Almacenes
                WHERE Nombre = @Nombre
            )
            THEN 1
            ELSE 0
        END
        AS BIT
    );
END;
GO

CREATE OR ALTER PROCEDURE sp_Almacen_ExistePorIdActivo
    @IdAlmacen BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CAST(
        CASE
            WHEN EXISTS (
                SELECT 1
                FROM Almacenes
                WHERE IdAlmacen = @IdAlmacen
                    AND Estado = 1
            )
            THEN 1
            ELSE 0
        END
        AS BIT
    );
END;
GO

CREATE OR ALTER PROCEDURE sp_Categoria_ExisteNombre
    @Nombre VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CAST(
        CASE
            WHEN EXISTS (
                SELECT 1
                FROM Categorias
                WHERE Nombre = @Nombre
            )
            THEN 1
            ELSE 0
        END
        AS BIT
    );
END;
GO

CREATE OR ALTER PROCEDURE sp_Categoria_ExistePorIdActivo
    @IdCategoria BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CAST(
        CASE
            WHEN EXISTS (
                SELECT 1
                FROM Categorias
                WHERE IdCategoria = @IdCategoria
                    AND Estado = 1
            )
            THEN 1
            ELSE 0
        END
        AS BIT
    );
END;
GO

CREATE OR ALTER PROCEDURE sp_Marca_ExisteNombre
    @Nombre VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CAST(
        CASE
            WHEN EXISTS (
                SELECT 1
                FROM Marcas
                WHERE Nombre = @Nombre
            )
            THEN 1
            ELSE 0
        END
        AS BIT
    );
END;
GO

CREATE OR ALTER PROCEDURE sp_Marca_ExistePorIdActivo
    @IdMarca BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CAST(
        CASE
            WHEN EXISTS (
                SELECT 1
                FROM Marcas
                WHERE IdMarca = @IdMarca
                    AND Estado = 1
            )
            THEN 1
            ELSE 0
        END
        AS BIT
    );
END;
GO

CREATE OR ALTER PROCEDURE sp_Proveedor_ExisteRuc
    @Ruc VARCHAR(11)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CAST(
        CASE
            WHEN EXISTS (
                SELECT 1
                FROM Proveedores
                WHERE Ruc = @Ruc
            )
            THEN 1
            ELSE 0
        END
        AS BIT
    );
END;
GO

CREATE OR ALTER PROCEDURE sp_Proveedor_ExisteCorreo
    @Correo VARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CAST(
        CASE
            WHEN EXISTS (
                SELECT 1
                FROM Proveedores
                WHERE Correo = @Correo
            )
            THEN 1
            ELSE 0
        END
        AS BIT
    );
END;
GO

CREATE OR ALTER PROCEDURE sp_Proveedor_ExistePorIdActivo
    @IdProveedor BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CAST(
        CASE
            WHEN EXISTS (
                SELECT 1
                FROM Proveedores
                WHERE IdProveedor = @IdProveedor
                    AND Estado = 1
            )
            THEN 1
            ELSE 0
        END
        AS BIT
    );
END;
GO

CREATE OR ALTER PROCEDURE sp_Usuario_ExisteCorreo
    @Correo VARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CAST(
        CASE
            WHEN EXISTS (
                SELECT 1
                FROM Usuarios
                WHERE Correo = @Correo
            )
            THEN 1
            ELSE 0
        END
        AS BIT
    );
END;
GO

CREATE OR ALTER PROCEDURE sp_Usuario_ExistePorIdActivo
    @IdUsuario BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CAST(
        CASE
            WHEN EXISTS (
                SELECT 1
                FROM Usuarios
                WHERE IdUsuario = @IdUsuario
                    AND Estado = 1
            )
            THEN 1
            ELSE 0
        END
        AS BIT
    );
END;
GO

CREATE OR ALTER PROCEDURE sp_Rol_ExistePorIdActivo
    @IdRol BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CAST(
        CASE
            WHEN EXISTS (
                SELECT 1
                FROM Roles
                WHERE IdRol = @IdRol
                    AND Estado = 1
            )
            THEN 1
            ELSE 0
        END
        AS BIT
    );
END;
GO

CREATE OR ALTER PROCEDURE sp_Producto_ExisteCodigo
    @Codigo VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CAST(
        CASE
            WHEN EXISTS (
                SELECT 1
                FROM Productos
                WHERE Codigo = @Codigo
            )
            THEN 1
            ELSE 0
        END
        AS BIT
    );
END;
GO

CREATE OR ALTER PROCEDURE sp_Producto_ExistePorIdActivo
    @IdProducto BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CAST(
        CASE
            WHEN EXISTS (
                SELECT 1
                FROM Productos
                WHERE IdProducto = @IdProducto
                    AND Estado = 1
            )
            THEN 1
            ELSE 0
        END
        AS BIT
    );
END;
GO

CREATE OR ALTER PROCEDURE sp_Inventario_ExisteProductoAlmacen
    @IdProducto BIGINT,
    @IdAlmacen BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CAST(
        CASE
            WHEN EXISTS (
                SELECT 1
                FROM Inventarios
                WHERE IdProducto = @IdProducto
                    AND IdAlmacen = @IdAlmacen
            )
            THEN 1
            ELSE 0
        END
        AS BIT
    );
END;
GO

CREATE OR ALTER PROCEDURE sp_TipoComprobante_ExistePorIdActivo
    @IdTipoComprobante BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CAST(
        CASE
            WHEN EXISTS (
                SELECT 1
                FROM TiposComprobante
                WHERE IdTipoComprobante = @IdTipoComprobante
                    AND Estado = 1
            )
            THEN 1
            ELSE 0
        END
        AS BIT
    );
END;
GO

CREATE OR ALTER PROCEDURE sp_Compra_ExisteComprobante
    @IdProveedor BIGINT,
    @IdTipoComprobante BIGINT,
    @NumeroComprobante VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CAST(
        CASE
            WHEN EXISTS (
                SELECT 1
                FROM Compras
                WHERE IdProveedor = @IdProveedor
                    AND IdTipoComprobante = @IdTipoComprobante
                    AND NumeroComprobante = @NumeroComprobante
            )
            THEN 1
            ELSE 0
        END
        AS BIT
    );
END;
GO
