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
public class EmpleadoService
{
    private readonly AppDbContext _context;
    public EmpleadoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<EmpleadoDto>> Consultar(string filtro)
    {
        var empleados = await
            _context.Empleados
            .Include(e => e.DatosPersonales)
            .Where(e => e.DatosPersonales.Nombre.Contains(filtro))
            .Select(
                e =>
                new EmpleadoDto()
                {
                    Id = e.Id,
                    PersonaId = e.PersonaId,
                    Sueldo = e.Sueldo,
                    Area = e.Area,
                    DatosPersonales = new PersonaDto()
                    {
                        Id = e.DatosPersonales.Id,
                        Nombre = e.DatosPersonales.Nombre
                    }
                }
            )
            .ToListAsync();
        return empleados;
    }
	public async Task<bool> Crear(EmpleadoDto request)
	{
		var empleado = Empleado.Create(
			request.DatosPersonales.Nombre,
			request.Sueldo,
			request.Area
		);
		_context.Empleados.Add(empleado);
		;
		return (await _context.SaveChangesAsync()) > 0;
	}
	public async Task<bool> Modificar(EmpleadoDto request)
	{
		//1. Busco el cliente
		var empleado = await _context.Empleados
			.Include(c => c.DatosPersonales)
			.FirstOrDefaultAsync(c => c.Id == request.Id);
		//2. Modifico el cliente
		empleado!.DatosPersonales.Nombre = request.DatosPersonales.Nombre;
		empleado!.Sueldo = request.Sueldo;
		empleado!.Area = request.Area;
		//Guardo los cambios
		return (await _context.SaveChangesAsync()) > 0;
	}
	public async Task<bool> Eliminar(int Id)
	{
		var empleado = await _context.Empleados
			.FirstOrDefaultAsync(c => c.Id == Id);

		_context.Empleados.Remove(empleado!);

		return (await _context.SaveChangesAsync()) > 0;
	}
}


