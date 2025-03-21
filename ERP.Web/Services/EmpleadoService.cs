using ERP.Web.Data;
using ERP.Web.Domain.Dto;
using ERP.Web.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace ERP.Web.Services;

public interface IEmpleadoService
{
	Task<List<EmpleadoDto>> Consultar(string filtro);
	Task<bool> Crear(EmpleadoDto request);
	Task<bool> Eliminar(int Id);
	Task<bool> Modificar(EmpleadoDto request);
}

public class EmpleadoService : IEmpleadoService
{
	private readonly AppDbContext _context;
	public EmpleadoService(AppDbContext context)
	{
		_context = context;
	}

	public async Task<List<EmpleadoDto>> Consultar(string filtro)
	{
		var empleado = await
			 _context.Empleados
			 .Include(em => em.DatosPersonales)
			 .Where(em => em.DatosPersonales.Nombre.Contains(filtro))
			 .Select(
				 em =>
				 new EmpleadoDto()
				 {
					 Id = em.Id,
					 PersonaId = em.PersonaId,
					 Sueldo = em.Sueldo,
					 Area = em.Area ?? "N/A",
					 DatosPersonales = new PersonaDto()
					 {
						 Id = em.DatosPersonales.Id,
						 Nombre = em.DatosPersonales.Nombre
					 }
				 }
			 )
			 .ToListAsync();
		return empleado;
	}
	public async Task<bool> Crear(EmpleadoDto request)
	{
		var empleado = Empleado.Create(
			request.DatosPersonales.Nombre,
			request.DatosPersonales.FechaDeNacimiento,
			request.Area ?? "Recursos humanos",
			request.Sueldo
			
		);
		_context.Empleados.Add(empleado);
		;
		return (await _context.SaveChangesAsync()) > 0;
	}
	public async Task<bool> Modificar(EmpleadoDto request)
	{
		//1. Busco el empleado
		var empleado = await _context.Empleados
			.Include(em => em.DatosPersonales)
			.FirstOrDefaultAsync(em => em.Id == request.Id);
		//2. Modifico el empleado
		empleado!.DatosPersonales.Nombre = request.DatosPersonales.Nombre;
		empleado!.DatosPersonales.FechaDeNacimiento = request.DatosPersonales.FechaDeNacimiento;
		empleado!.Area = request.Area;
		empleado!.Sueldo = request.Sueldo;
		//Guardo los cambios
		return (await _context.SaveChangesAsync()) > 0;
	}
	public async Task<bool> Eliminar(int Id)
	{
		var empleado = await _context.Empleados
			.FirstOrDefaultAsync(em => em.Id == Id);

		_context.Empleados.Remove(empleado!);

		return (await _context.SaveChangesAsync()) > 0;
	}
}