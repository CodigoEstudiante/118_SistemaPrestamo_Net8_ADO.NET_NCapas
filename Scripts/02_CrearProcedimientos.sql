use DBPrestamo

go
--- CONFIGURACION ---

create FUNCTION [dbo].[SplitString]  ( 
	@string NVARCHAR(MAX), 
	@delimiter CHAR(1)  
)
RETURNS
@output TABLE(valor NVARCHAR(MAX)  ) 
BEGIN 
	DECLARE @start INT, @end INT 
	SELECT @start = 1, @end = CHARINDEX(@delimiter, @string) 
	WHILE @start < LEN(@string) + 1
	BEGIN 
		IF @end = 0  
        SET @end = LEN(@string) + 1 

		INSERT INTO @output (valor)  
		VALUES(SUBSTRING(@string, @start, @end - @start)) 
		SET @start = @end + 1 
		SET @end = CHARINDEX(@delimiter, @string, @start) 
	END 
	RETURN
END

go

create procedure sp_obtenerUsuario(
@Correo varchar(50),
@Clave varchar(50)
)
as
begin
	select IdUsuario,NombreCompleto,Correo from Usuario where 
	Correo = @Correo COLLATE SQL_Latin1_General_CP1_CS_AS and
	Clave = @Clave COLLATE SQL_Latin1_General_CP1_CS_AS
end

go

-- PROCEDMIENTOS PARA MONEDA 

create procedure sp_listaMoneda
as
begin
	select IdMoneda,Nombre,Simbolo,convert(char(10),FechaCreacion,103)[FechaCreacion] from Moneda
end

go

create procedure sp_crearMoneda(
@Nombre varchar(50),
@Simbolo varchar(50),
@msgError varchar(100) OUTPUT
)
as
begin

	set @msgError = ''
	if(not exists(select * from Moneda where 
		Nombre = @Nombre COLLATE SQL_Latin1_General_CP1_CS_AS
	))
		insert into Moneda(Nombre,Simbolo) values(@Nombre,@Simbolo)
	else
		set @msgError = 'La moneda ya existe'
end

go

create procedure sp_editarMoneda(
@IdMoneda int,
@Nombre varchar(50),
@Simbolo varchar(50),
@msgError varchar(100) OUTPUT
)
as
begin

	set @msgError = ''
	if(not exists(select * from Moneda where 
		Nombre = @Nombre COLLATE SQL_Latin1_General_CP1_CS_AS and
		IdMoneda != @IdMoneda
	))
		update Moneda set Nombre = @Nombre ,Simbolo = @Simbolo where IdMoneda = @IdMoneda
	else
		set @msgError = 'La moneda ya existe'
end

go

create procedure sp_eliminarMoneda(
@IdMoneda int,
@msgError varchar(100) OUTPUT
)
as
begin

	set @msgError = ''
	if(not exists(select IdPrestamo from Prestamo where IdMoneda = @IdMoneda))
		delete from Moneda where IdMoneda = @IdMoneda
	else
		set @msgError = 'La moneda esta utilizada en un prestamo, no se puede eliminar'
end

go

-- PROCEDMIENTOS PARA CLIENTE

create procedure sp_listaCliente
as
begin
	select IdCliente,NroDocumento,Nombre,Apellido,Correo,Telefono,convert(char(10),FechaCreacion,103)[FechaCreacion] from Cliente
end

go

create procedure sp_obtenerCliente(
@NroDocumento varchar(50)
)
as
begin
	select IdCliente,NroDocumento,Nombre,Apellido,Correo,Telefono,convert(char(10),FechaCreacion,103)[FechaCreacion] from Cliente
	where NroDocumento = @NroDocumento
end

go

create procedure sp_crearCliente(
@NroDocumento varchar(50),
@Nombre varchar(50),
@Apellido varchar(50),
@Correo varchar(50),
@Telefono varchar(50),
@msgError varchar(100) OUTPUT
)
as
begin

	set @msgError = ''
	if(not exists(select * from Cliente where 
		NroDocumento = @NroDocumento
	))
		insert into Cliente(NroDocumento,Nombre,Apellido,Correo,Telefono) values
		(@NroDocumento,@Nombre,@Apellido,@Correo,@Telefono)
	else
		set @msgError = 'El cliente ya existe'
end

go


create procedure sp_editarCliente(
@IdCliente int,
@NroDocumento varchar(50),
@Nombre varchar(50),
@Apellido varchar(50),
@Correo varchar(50),
@Telefono varchar(50),
@msgError varchar(100) OUTPUT
)
as
begin

	set @msgError = ''
	if(not exists(select * from Cliente where 
		NroDocumento = @NroDocumento and IdCliente != @IdCliente
	))
		update Cliente set NroDocumento = @NroDocumento,Nombre = @Nombre,Apellido = @Apellido,Correo = @Correo,Telefono = @Telefono 
		where IdCliente = @IdCliente
	else
		set @msgError = 'El cliente ya existe'
end

go

create procedure sp_eliminarCliente(
@IdCliente int,
@msgError varchar(100) OUTPUT
)
as
begin

	set @msgError = ''
	if(not exists(select IdPrestamo from Prestamo where IdCliente = @IdCliente))
		delete from Cliente where IdCliente = @IdCliente
	else
		set @msgError = 'El cliente tiene historial de prestamo, no se puede eliminar'
end

go


-- PROCEDIMIENTOS PARA PRESTAMOS

create procedure sp_crearPrestamo(
@IdCliente int,
@NroDocumento varchar(50),
@Nombre varchar(50),
@Apellido varchar(50),
@Correo varchar(50),
@Telefono varchar(50),
@IdMoneda int,
@FechaInicio varchar(50),
@MontoPrestamo varchar(50),
@InteresPorcentaje varchar(50),
@NroCuotas int,
@FormaDePago varchar(50),
@ValorPorCuota varchar(50),
@ValorInteres varchar(50),
@ValorTotal varchar(50),
@msgError varchar(100) OUTPUT
)
as
begin
	set dateformat dmy
	set @msgError = ''

	begin try

		declare @FecInicio date = convert(date,@FechaInicio)
		declare @MontPrestamo decimal(10,2) = convert(decimal(10,2),@MontoPrestamo)
		declare @IntPorcentaje decimal(10,2) = convert(decimal(10,2),@InteresPorcentaje)
		declare @VlrPorCuota decimal(10,2) = convert(decimal(10,2),@ValorPorCuota)
		declare @VlrInteres decimal(10,2) = convert(decimal(10,2),@ValorInteres)
		declare @VlrTotal decimal(10,2) = convert(decimal(10,2),@ValorTotal)
		create table #TempIdentity(Id int,Nombre varchar(10))

		begin transaction

		if(@IdCliente = 0)
		begin
			insert into Cliente(NroDocumento,Nombre,Apellido,Correo,Telefono)
			OUTPUT INSERTED.IdCliente,'Cliente' INTO #TempIdentity(Id,Nombre)
			values
			(@NroDocumento,@Nombre,@Apellido,@Correo,@Telefono)

			set @IdCliente = (select Id from #TempIdentity where Nombre = 'Cliente')
		end
		else
		begin
			if(exists(select * from Prestamo where IdCliente = @IdCliente and Estado = 'Pendiente'))
				set @msgError = 'El cliente tiene un prestamo pendiente, debe cancelar el anterior'
		end

		if(@msgError ='')
		begin

			insert into Prestamo(IdCliente,IdMoneda,FechaInicioPago,MontoPrestamo,InteresPorcentaje,NroCuotas,FormaDePago,ValorPorCuota,ValorInteres,ValorTotal,Estado)
			OUTPUT INSERTED.IdPrestamo,'Prestamo' INTO #TempIdentity(Id,Nombre)
			values
			(@IdCliente,@IdMoneda,@FecInicio,@MontPrestamo,@IntPorcentaje,@NroCuotas,@FormaDePago,@VlrPorCuota,@VlrInteres,@VlrTotal,'Pendiente')

			;with detalle(IdPrestamo,FechaPago,NroCuota,MontoCuota,Estado) as
			(
				select (select Id from #TempIdentity where Nombre = 'Prestamo'),@FecInicio,0,@VlrPorCuota,'Pendiente'
				union all
				select IdPrestamo,
				case @FormaDePago 
					when 'Diario' then DATEADD(day,1,FechaPago)
					when 'Semanal' then DATEADD(WEEK,1,FechaPago)
					when 'Quincenal' then DATEADD(day,15,FechaPago)
					when 'Mensual' then DATEADD(MONTH,1,FechaPago)
				end,
				NroCuota + 1,MontoCuota,Estado from detalle
				where NroCuota < @NroCuotas
			)
			select IdPrestamo,FechaPago,NroCuota,MontoCuota,Estado into #tempDetalle from detalle where NroCuota > 0
	
			insert into PrestamoDetalle(IdPrestamo,FechaPago,NroCuota,MontoCuota,Estado)
			select IdPrestamo,FechaPago,NroCuota,MontoCuota,Estado from #tempDetalle

		end

		commit transaction
	end try
	begin catch
		rollback transaction
		set @msgError = ERROR_MESSAGE()
	end catch
	
end

go

create procedure sp_obtenerPrestamos(
@IdPrestamo int = 0,
@NroDocumento varchar(50) = ''
)as
begin
	select p.IdPrestamo,
	c.IdCliente,c.NroDocumento,c.Nombre,c.Apellido,c.Correo,c.Telefono,
	m.IdMoneda,m.Nombre[NombreMoneda],m.Simbolo,
	CONVERT(char(10),p.FechaInicioPago, 103) [FechaInicioPago],
	CONVERT(VARCHAR,p.MontoPrestamo)[MontoPrestamo],
	CONVERT(VARCHAR,p.InteresPorcentaje)[InteresPorcentaje],
	p.NroCuotas,
	p.FormaDePago,
	CONVERT(VARCHAR,p.ValorPorCuota)[ValorPorCuota],
	CONVERT(VARCHAR,p.ValorInteres)[ValorInteres],
	CONVERT(VARCHAR,p.ValorTotal)[ValorTotal],
	p.Estado,
	CONVERT(char(10),p.FechaCreacion, 103) [FechaCreacion],
	(
		select pd.IdPrestamoDetalle,CONVERT(char(10),pd.FechaPago, 103) [FechaPago],
		CONVERT(VARCHAR,pd.MontoCuota)[MontoCuota],
		pd.NroCuota,pd.Estado,isnull(CONVERT(varchar(10),pd.FechaPagado, 103),'')[FechaPagado]
		from PrestamoDetalle pd
		where pd.IdPrestamo = p.IdPrestamo
		FOR XML PATH('Detalle'), TYPE, ROOT('PrestamoDetalle')
	)
	from Prestamo p
	inner join Cliente c on c.IdCliente = p.IdCliente
	inner join Moneda m on m.IdMoneda = p.IdMoneda
	where p.IdPrestamo = iif(@IdPrestamo = 0,p.idprestamo,@IdPrestamo) and
	c.NroDocumento = iif(@NroDocumento = '',c.NroDocumento,@NroDocumento)
	FOR XML PATH('Prestamo'), ROOT('Prestamos'), TYPE;
end


go
  
create procedure sp_pagarCuotas(  
@IdPrestamo int,  
@NroCuotasPagadas varchar(100),  
@msgError varchar(100) OUTPUT  
)  
as  
begin  
  
 set dateformat dmy  
 set @msgError = ''  
  
 begin try  
  
  begin transaction  
  
   update pd set pd.Estado = 'Cancelado', FechaPagado = getdate() from PrestamoDetalle pd  
   inner join dbo.SplitString(@NroCuotasPagadas,',') ss on ss.valor = pd.NroCuota  
   where IdPrestamo = @IdPrestamo  
  
   if((select count(IdPrestamoDetalle) from PrestamoDetalle where IdPrestamo = @IdPrestamo and Estado='Pendiente') = 0)  
   begin  
    update Prestamo set Estado = 'Cancelado' where IdPrestamo = @IdPrestamo  
   end  
  
  
  commit transaction  
 end try  
 begin catch  
  rollback transaction  
  set @msgError = ERROR_MESSAGE()  
 end catch  
  
end  
-- PROCEDIMIENTO PARA RESUMEN

GO

create PROCEDURE sp_obtenerResumen
as
begin
select 
(select convert(varchar,count(*)) from Cliente) [TotalClientes],
(select convert(varchar,count(*)) from Prestamo where Estado = 'Pendiente')[PrestamosPendientes],
(select convert(varchar,count(*)) from Prestamo where Estado = 'Cancelado')[PrestamosCancelados],
(select convert(varchar,isnull(sum(ValorInteres),0)) from Prestamo where Estado = 'Cancelado')[InteresAcumulado]
end

GO