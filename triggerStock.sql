CREATE TRIGGER ActualizarStock_Venta
ON detalle_venta
FOR INSERT
AS UPDATE a SET a.stock = a.stock - d.cantidad 
FROM articulo AS a INNER JOIN
inserted AS d ON d.idarticulo = a.idarticulo