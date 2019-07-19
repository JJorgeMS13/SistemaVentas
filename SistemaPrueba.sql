--Select * from categoria with(nolock)

--Select * from articulo with(nolock)

--Select * from rol with(nolock)

--Select * from usuario with(nolock)

--Select * from persona with(nolock)

--Select * from ingreso with(nolock)

--Select * from detalle_ingreso with(nolock)

Select * from venta 

Select * from detalle_venta

--UPDATE V SET V.fecha_hora='2019-09-19 11:26:29.630' FROM venta V 
--WHERE idventa = 2

--Insert into persona(tipo_persona,nombre,tipo_documento,num_documento,direccion,telefono,email)
--values('Proveedor','Juan Mendez','DNI','Jn12345','Centro Guadalajara','333895681','JUAN.01@HOTMAIL.COM'),
--('Cliente','José Morales','DNI','Js12345','Centro Puebla','7971123569','JOSE.01@HOTMAIL.COM')
--insert into rol(nombre,descripcion,condicion)
--values('Administrador','Administardor del sistema',1),
--('Almacenero','Almacenero del sistema',1),
--('Vendedor','Vendedor del sistema',1)
--insert into articulo(idcategoria,codigo,nombre,precio_venta,stock,descripcion,condicion)
--values(2,'Com123','Toshiba',9000,15,'Toshiba E0498me',1)

--ALTER TABLE usuario ALTER COLUMN password_salt varbinary (MAX); 