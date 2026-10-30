USE Sistema_Gestion;
GO

DECLARE @Juan INT;
DECLARE @Maria INT;
DECLARE @Roberto INT;

SELECT @Juan = id_usuario FROM dbo.Usuario WHERE usuario = 'jperez';
SELECT @Maria = id_usuario FROM dbo.Usuario WHERE usuario = 'mgomez';
SELECT @Roberto = id_usuario FROM dbo.Usuario WHERE usuario = 'rlopez';
DECLARE @IdPresupuesto INT;
DECLARE @IdNota INT;
DECLARE @IdLinea INT;
DECLARE @IdAviso INT;
DECLARE @Hace2Dias DATETIME2(0);
DECLARE @Hace1Dia DATETIME2(0);

SET @Hace2Dias = DATEADD(DAY, -2, GETDATE());
SET @Hace1Dia = DATEADD(DAY, -1, GETDATE());

EXEC dbo.sp_PresupuestoVenta_Alta 1, @Juan, @Hace2Dias, @IdPresupuesto OUTPUT;
EXEC dbo.sp_DetallePresupuestoVenta_Alta @IdPresupuesto, 1, 10, 45000.00, @IdLinea OUTPUT;
EXEC dbo.sp_DetallePresupuestoVenta_Alta @IdPresupuesto, 2, 2, 320000.00, @IdLinea OUTPUT;
EXEC dbo.sp_PresupuestoVenta_CambiarEstado @IdPresupuesto, 'Aprobado';
EXEC dbo.sp_NotaPedido_GenerarDesdePresupuesto @IdPresupuesto, @Juan, @IdNota OUTPUT;

EXEC dbo.sp_PresupuestoVenta_Alta 2, @Maria, @Hace1Dia, @IdPresupuesto OUTPUT;
EXEC dbo.sp_DetallePresupuestoVenta_Alta @IdPresupuesto, 5, 4, 210000.00, @IdLinea OUTPUT;

EXEC dbo.sp_PresupuestoVenta_Alta 3, @Juan, @Hace1Dia, @IdPresupuesto OUTPUT;
EXEC dbo.sp_DetallePresupuestoVenta_Alta @IdPresupuesto, 7, 1, 890000.00, @IdLinea OUTPUT;
EXEC dbo.sp_PresupuestoVenta_CambiarEstado @IdPresupuesto, 'Rechazado';

EXEC dbo.sp_NotaPedido_Alta 4, @Maria, @Hace2Dias, @IdNota OUTPUT;
EXEC dbo.sp_DetalleNotaPedido_Alta @IdNota, 6, 1, 260000.00, @IdLinea OUTPUT;
EXEC dbo.sp_DetalleNotaPedido_Alta @IdNota, 8, 3, 68000.00, @IdLinea OUTPUT;
EXEC dbo.sp_Aviso_Alta @IdNota, @Roberto, @Hace1Dia, @IdAviso OUTPUT;
EXEC dbo.sp_NotaPedido_CambiarEstado @IdNota, 'Entregado';

EXEC dbo.sp_NotaPedido_Alta 3, @Juan, @Hace1Dia, @IdNota OUTPUT;
EXEC dbo.sp_DetalleNotaPedido_Alta @IdNota, 3, 2, 150000.00, @IdLinea OUTPUT;

PRINT 'Datos de prueba cargados.';
GO

DECLARE @Juan INT;
DECLARE @Maria INT;
DECLARE @Roberto INT;

SELECT @Juan = id_usuario FROM dbo.Usuario WHERE usuario = 'jperez';
SELECT @Maria = id_usuario FROM dbo.Usuario WHERE usuario = 'mgomez';
SELECT @Roberto = id_usuario FROM dbo.Usuario WHERE usuario = 'rlopez';
DECLARE @Id INT;

BEGIN TRY
    EXEC dbo.sp_DetallePresupuestoVenta_Alta 2, 1, 0, 45000.00, @Id OUTPUT;
    PRINT 'PRUEBA 1 FALLIDA: se aceptó una línea con cantidad 0.';
END TRY
BEGIN CATCH
    PRINT 'PRUEBA 1 OK (rechazo esperado): ' + ERROR_MESSAGE();
END CATCH;

BEGIN TRY
    EXEC dbo.sp_PresupuestoVenta_CambiarEstado 1, 'Rechazado';
    PRINT 'PRUEBA 2 FALLIDA: se cambió el estado de un presupuesto ya aprobado.';
END TRY
BEGIN CATCH
    PRINT 'PRUEBA 2 OK (rechazo esperado): ' + ERROR_MESSAGE();
END CATCH;

BEGIN TRY
    EXEC dbo.sp_NotaPedido_GenerarDesdePresupuesto 2, @Juan, @Id OUTPUT;
    PRINT 'PRUEBA 3 FALLIDA: se generó un pedido desde un presupuesto Pendiente.';
END TRY
BEGIN CATCH
    PRINT 'PRUEBA 3 OK (rechazo esperado): ' + ERROR_MESSAGE();
END CATCH;

BEGIN TRY
    EXEC dbo.sp_NotaPedido_GenerarDesdePresupuesto 1, @Juan, @Id OUTPUT;
    PRINT 'PRUEBA 4 FALLIDA: se generaron dos pedidos desde el mismo presupuesto.';
END TRY
BEGIN CATCH
    PRINT 'PRUEBA 4 OK (rechazo esperado): ' + ERROR_MESSAGE();
END CATCH;

BEGIN TRY
    EXEC dbo.sp_PresupuestoVenta_Baja 1;
    PRINT 'PRUEBA 5 FALLIDA: se dio de baja un presupuesto aprobado.';
END TRY
BEGIN CATCH
    PRINT 'PRUEBA 5 OK (rechazo esperado): ' + ERROR_MESSAGE();
END CATCH;

BEGIN TRY
    EXEC dbo.sp_NotaPedido_CambiarEstado 3, 'Entregado';
    PRINT 'PRUEBA 6 FALLIDA: un pedido Pendiente pasó directo a Entregado.';
END TRY
BEGIN CATCH
    PRINT 'PRUEBA 6 OK (rechazo esperado): ' + ERROR_MESSAGE();
END CATCH;

BEGIN TRY
    EXEC dbo.sp_Aviso_Alta 2, @Roberto, NULL, @Id OUTPUT;
    PRINT 'PRUEBA 7 FALLIDA: se avisó la preparación de un pedido que no estaba Pendiente.';
END TRY
BEGIN CATCH
    PRINT 'PRUEBA 7 OK (rechazo esperado): ' + ERROR_MESSAGE();
END CATCH;

BEGIN TRY
    EXEC dbo.sp_PresupuestoVenta_Alta 5, @Juan, '2026-01-01', @Id OUTPUT;
    PRINT 'PRUEBA 8 FALLIDA: se le hizo un presupuesto a un cliente dado de baja.';
END TRY
BEGIN CATCH
    PRINT 'PRUEBA 8 OK (rechazo esperado): ' + ERROR_MESSAGE();
END CATCH;
GO

EXEC dbo.sp_PresupuestoVenta_Listar;
EXEC dbo.sp_NotaPedido_Listar;
EXEC dbo.sp_Aviso_Listar;
EXEC dbo.sp_Rpt_PresupuestosVentaPorClienteEstado;
EXEC dbo.sp_Rpt_NotasPedidoPorEstado;
EXEC dbo.sp_Rpt_PedidosPendientesPreparar;
EXEC dbo.sp_Rpt_AvisosPorPeriodo;
EXEC dbo.sp_Rpt_TiempoPromedioPreparacion;
GO
