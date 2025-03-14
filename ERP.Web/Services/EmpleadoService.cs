using ERP.Web.Data;
using ERP.Web.Domain.Dto;
using ERP.Web.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace ERP.Web.Services;

public interface IEmpleadoService
{

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
                        Nombre = e.DatosPersonales.Nombre,
                        FechaDeNacimiento = e.DatosPersonales.FechaDeNacimiento
                    }
                }
            )
            .ToListAsync();
        return empleados;
    }
}
