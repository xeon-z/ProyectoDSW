Use Master
go

if exists(Select * from sys.databases  Where name='Cosmetica')
Begin
	Drop Database Cosmetica
End
go

create database Cosmetica
go


IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Cosmetica].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [Cosmetica] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [Cosmetica] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [Cosmetica] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [Cosmetica] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [Cosmetica] SET ARITHABORT OFF 
GO

ALTER DATABASE [Cosmetica] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [Cosmetica] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [Cosmetica] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [Cosmetica] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [Cosmetica] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [Cosmetica] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [Cosmetica] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [Cosmetica] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [Cosmetica] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [Cosmetica] SET  ENABLE_BROKER 
GO

ALTER DATABASE [Cosmetica] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [Cosmetica] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [Cosmetica] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [Cosmetica] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [Cosmetica] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [Cosmetica] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [Cosmetica] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [Cosmetica] SET RECOVERY FULL 
GO

ALTER DATABASE [Cosmetica] SET  MULTI_USER 
GO

ALTER DATABASE [Cosmetica] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [Cosmetica] SET DB_CHAINING OFF 
GO

ALTER DATABASE [Cosmetica] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [Cosmetica] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO

ALTER DATABASE [Cosmetica] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [Cosmetica] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO

ALTER DATABASE [Cosmetica] SET QUERY_STORE = OFF
GO

ALTER DATABASE [Cosmetica] SET  READ_WRITE 
GO

USE Cosmetica
go

set dateformat ymd
go

CREATE TABLE Categorias (
  IdCategoria int primary key ,
  NombreCategoria varchar(90) not null,
)
go
--registro de categorias
INSERT INTO Categorias  VALUES(1, 'Maquillaje')
INSERT INTO Categorias  VALUES(2, 'Perfumes')
INSERT INTO Categorias  VALUES(3, 'Cuidado Corporal')
INSERT INTO Categorias  VALUES(4, 'Cabello')
INSERT INTO Categorias  VALUES(5, 'Accesorios')
INSERT INTO Categorias  VALUES(6, 'Aceites')
INSERT INTO Categorias  VALUES(7, 'Cuidado de la Piel')
INSERT INTO Categorias  VALUES(8, 'Hombres')
INSERT INTO Categorias  VALUES(9, 'Ni?os y Bebe')
go

CREATE TABLE Usuarios (
  Dni varchar (8) primary key,
  Usuario varchar(30),
  Clave varchar(40) not null,
  NombreCompleto varchar(60) not null,
  Rol char(1) not null,
  intentos int default(0),
  fecBloque Datetime null
)
go

insert Usuarios Values('78020954','enzo','1234','Enzo Robles','1',0,null)
insert Usuarios Values('07245621','mariabecerra@gmail.com','maria1','Maria Becerra','2',0,null)
insert Usuarios Values('71829365','john','666','John Yupari','1',0,null)
go



CREATE TABLE Clientes (
  Dni varchar (8) primary key references Usuarios,
  ApeCliente varchar(40) not null,
  NomCliente varchar(40) not null,
  Direccion varchar(60) not null,
  Distrito varchar(60) not null,
  Telefono varchar(24) not null
)
go

INSERT INTO Clientes 
VALUES('07245621', 'Maria', 'Becerra', 'Av jos? Paz 148',' Lince','957589535'),
VALUES('71829365', 'Maria', 'Becerra', 'Av jos? Paz 148',' Lince','957589535')
go

CREATE TABLE Empleados (
  Dni varchar (8) primary key references Usuarios,
  ApeEmpleado varchar(50) not null,
  NomEmpleado varchar(50) not null,
  FecNac datetime not null,
  DirEmpleado varchar(60) not null,
  fonoEmpleado varchar(15) NULL,
  cargo varchar(15),
  FecContrata datetime not null
)
go

INSERT INTO Empleados VALUES
('78020954', 'Robles', 'Enzo', '2002-01-30 00:00:00','Av Arequpa 1590','994954489','administrador','2010-02-10')
go


CREATE TABLE Productos (
  IdProducto int primary key IDENTITY(1,1),
  NombreProducto varchar(40) not null,
  IdCategoria int References Categorias,
  PrecioUnidad decimal(10,0) not null,
  UnidadesEnExistencia smallint not null)
go

--registro de productos
INSERT INTO Productos VALUES( 'Labial Rosado con brillo', '1' ,'15', '39')
INSERT INTO Productos VALUES('Desodorante en Aerosol OLD SPICE 150ml', '8' ,'12.90', '27')
INSERT INTO Productos VALUES( 'Shampoo Head & Shoulders frasco 375 ML', '4' ,'15.90', '15')
go
--
CREATE TABLE tb_pedidos(
	idpedido int primary key,
	fpedido datetime default(getdate()),
	dni varchar(8),
	nombre varchar(255),
	email varchar(255),
)
GO
--
create table tb_pedidos_deta(
	idpedido int references tb_pedidos,
	idproducto int references Productos,
	cantidad int,
	precio decimal
)
GO
--Funcion
create or alter function dbo.autogenera() returns int
As
Begin
	Declare @n int = (Select top 1 idpedido from tb_pedidos order by 1 desc)
	if(@n is null)
		set @n = 1
	else 
		set @n+=1
	return @n
End
--
go

select * from Productos
go

select * from Clientes
go

Select * from Empleados
go

Select * from Categorias
go

Select * from Usuarios
go

----------------------------------------------------------------------------

--Procedures
--Login

create or alter proc usp_acceso_usuario
@usuario varchar(30),
@clave varchar(10),
@fullname varchar(255) output,
@sw int output
As
Begin
	Set @fullname=(Select CONCAT(e.NomEmpleado, space(1),e.ApeEmpleado)
			from Usuarios as u join Empleados as e on u.Dni=e.Dni 
			where Clave=@clave and Usuario=@usuario)
	if(@fullname is null)
		Begin
			Set @fullname='Usuario o Clave Incorrecta'
			Set @sw=0
		End
	else
		Begin
			Set @sw=1
		End
End
go


declare @full varchar(255), @sw int
exec usp_acceso_usuario 'enzo','1234',@full output, @sw output
print @full
go
--listado
/*
  IdProducto int primary key,
  NombreProducto varchar(40) not null,
  IdCategoria int References Categorias,
  PrecioUnidad decimal(10,0) not null,
  UnidadesEnExistencia smallint not null)*/

create or alter proc usp_productos
as
select IdProducto, NombreProducto, NombreCategoria, PrecioUnidad, UnidadesEnExistencia from Productos
p join Categorias as c on p.IdCategoria= c.IdCategoria
go

create or alter proc usp_filtrar_productos
@nombre varchar(60)
as
select IdProducto, NombreProducto, NombreCategoria PrecioUnidad, UnidadesEnExistencia from Productos
p join Categorias as c on p.IdCategoria= c.IdCategoria
where NombreProducto LIKE @nombre+'%'
go
--
create or alter proc usp_categorias
as
select* from Categorias
go
exec usp_categorias
go
--
create or alter proc usp_registrar_producto
@Nombre varchar(40),
@IdCat int ,
@Pre decimal(10,0),
@stock smallint
as
insert into Productos 
values (@Nombre, @IdCat, @Pre, @stock)
go
--
create or alter proc usp_editar_producto
@id int,
@Nombre varchar(40),
@IdCat int ,
@Pre decimal(10,0),
@stock smallint
as
update Productos 
set NombreProducto= @Nombre, IdCategoria=@IdCat, PrecioUnidad=@Pre, UnidadesEnExistencia=@stock
where IdProducto=@id
go
--
create or alter proc usp_eliminar_producto
@id int
as
delete Productos 
where IdProducto=@id
go
--
create or alter proc usp_agrega_pedido
@idpedido int output,
@dni varchar(8),
@nombre varchar(255),
@email varchar(255)
as
begin 
	set @idpedido = dbo.autogenera()
	insert tb_pedidos(idpedido,dni,nombre,email) values(@idpedido,@dni,@nombre,@email)
end
go
--
create or alter proc usp_agrega_detalle
@idpedido int,
@idproducto int,
@cantidad int,
@precio decimal
as
	insert tb_pedidos_deta values(@idpedido,@idproducto,@cantidad,@precio)
go
--
create or alter proc usp_actualiza_stock
@idproducto int,
@cant smallint
as
	Update Productos set UnidadesEnExistencia-=@cant where IdProducto = @idproducto
go


-- Avance 09/11/2022--


create or alter proc usp_acceso_usuario
@usuario varchar(30),
@clave varchar(10),
@fullname varchar(255) output,
@sw int output,
@rol char(1)
As
Begin
	Set @fullname=(Select CONCAT(e.NomEmpleado, space(1),e.ApeEmpleado)
			from Usuarios as u join Empleados as e on u.Dni=e.Dni 
			where Clave=@clave and Usuario=@usuario)
	if(@fullname is null)
		Begin
			Set @fullname='Usuario o Clave Incorrecta'
			Set @sw=0
		End
	else
		Begin
			Set @rol=(Select U.Rol
					from Usuarios as u join Empleados as e on u.Dni=e.Dni 
					where Clave=@clave and Usuario=@usuario)
				if(@rol=1)
					Begin
						Set @sw=2
					End
				else
					Begin
						Set @sw=1
					End
			End
End
go

create or alter proc usp_acceso_usuario
@usuario varchar(30),
@clave varchar(10),
@fullname varchar(255) output,
@sw int output,
@rol char(1)
As
Begin
	Set @fullname=(Select NombreCompleto
			from Usuarios
			where Clave=@clave and Usuario=@usuario)
	if(@fullname is null)
		Begin
			Set @fullname='Usuario o Clave Incorrecta'
			Set @sw=0
		End
	else
		Begin
			Set @rol=(Select U.Rol
					from Usuarios as u join Empleados as e on u.Dni=e.Dni 
					where Clave=@clave and Usuario=@usuario)
				if(@rol=1)
					Begin
						Set @sw=2
					End
				else
					Begin
						Set @sw=1
					End
			End
End
go

-- Ejemplos:
declare @nam varchar(255), @sw int
exec usp_acceso_usuario 'enzo','1234',@nam output, @sw output,''
print @sw
print @nam
go

declare @nam varchar(255), @sw int
exec usp_acceso_usuario 'john','666',@nam output, @sw output,''
print @sw
print @nam
go

declare @nam varchar(255), @sw int
exec usp_acceso_usuario 'mariabecerra@gmail.com','maria1',@nam output, @sw output,''
print @sw
print @nam
go