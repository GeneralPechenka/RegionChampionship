using AutoMapper;
using Database;
using Domain.Entities;
using DTOs.Core.Maintenance;
using DTOs.General;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/Core/[controller]")]
[ApiController]
public class MaintenancesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<MaintenancesController> _logger;

    public MaintenancesController(
        AppDbContext context,
        IMapper mapper,
        ILogger<MaintenancesController> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetMaintenances(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var query = _context.Maintenances
                .Include(m => m.VendingMachine)
                .Include(m => m.Employee)
                .AsNoTracking();

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(m => m.MaintenanceDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = _mapper.Map<List<MaintenanceShortDto>>(items);
            var response = new PagedResponse<MaintenanceShortDto>
            {
                Items = result,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении списка обслуживаний");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMaintenance(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                return BadRequest("Неверный идентификатор");

            var maintenance = await _context.Maintenances
                .Include(m => m.VendingMachine)
                .Include(m => m.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (maintenance == null)
                return NotFound($"Обслуживание с ID {id} не найдено");

            var dto = _mapper.Map<MaintenanceDetailsDto>(maintenance);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении обслуживания с ID {Id}", id);
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateMaintenance([FromBody] CreateUpdateMaintenanceDto dto)
    {
        try
        {
            if (dto == null)
                return BadRequest("Данные не предоставлены");

            // Проверка ТА
            var vendingMachine = await _context.VendingMachines.FindAsync(dto.VendingMachineId);
            if (vendingMachine == null)
                return BadRequest("Вендинговый аппарат не найден");

            // Проверка сотрудника
            if (dto.EmployeeId.HasValue)
            {
                var employee = await _context.Employees.FindAsync(dto.EmployeeId.Value);
                if (employee == null)
                    return BadRequest("Сотрудник не найден");
            }

            var maintenance = _mapper.Map<Maintenance>(dto);
            maintenance.Id = Guid.NewGuid();

            _context.Maintenances.Add(maintenance);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMaintenance),
                new { id = maintenance.Id },
                new { Id = maintenance.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании обслуживания");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMaintenance(Guid id, [FromBody] CreateUpdateMaintenanceDto dto)
    {
        try
        {
            if (id == Guid.Empty)
                return BadRequest("Неверный идентификатор");

            var maintenance = await _context.Maintenances.FindAsync(id);
            if (maintenance == null)
                return NotFound($"Обслуживание с ID {id} не найдено");

            // Проверка ТА
            var vendingMachine = await _context.VendingMachines.FindAsync(dto.VendingMachineId);
            if (vendingMachine == null)
                return BadRequest("Вендинговый аппарат не найден");

            // Проверка сотрудника
            if (dto.EmployeeId.HasValue && dto.EmployeeId.Value != maintenance.EmployeeId)
            {
                var employee = await _context.Employees.FindAsync(dto.EmployeeId.Value);
                if (employee == null)
                    return BadRequest("Сотрудник не найден");
            }

            _mapper.Map(dto, maintenance);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (DbUpdateConcurrencyException)
        {
            _logger.LogWarning("Конфликт параллельного доступа при обновлении обслуживания {Id}", id);
            return Conflict("Данные были изменены другим пользователем");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении обслуживания {Id}", id);
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMaintenance(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                return BadRequest("Неверный идентификатор");

            var maintenance = await _context.Maintenances.FindAsync(id);
            if (maintenance == null)
                return NotFound($"Обслуживание с ID {id} не найдено");

            _context.Maintenances.Remove(maintenance);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении обслуживания {Id}", id);
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }
}