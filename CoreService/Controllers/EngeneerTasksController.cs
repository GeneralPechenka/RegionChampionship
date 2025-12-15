using AutoMapper;
using Database;
using Domain.Entities;
using Domain.Enums;
using DTOs.Core.Maintenance;
using DTOs.General;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/Core/[controller]")]
[ApiController]
public class EngineerTasksController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<EngineerTasksController> _logger;

    public EngineerTasksController(
        AppDbContext context,
        IMapper mapper,
        ILogger<EngineerTasksController> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetEngineerTasks(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var query = _context.EngineerTasks
                .Include(t => t.VendingMachine)
                .Include(t => t.AssignedTo)
                .AsNoTracking();

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(t => t.DueDate)
                .ThenByDescending(t => t.Priority)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = _mapper.Map<List<EngineerTaskShortDto>>(items);
            var response = new PagedResponse<EngineerTaskShortDto>
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
            _logger.LogError(ex, "Ошибка при получении списка задач");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEngineerTask(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                return BadRequest("Неверный идентификатор");

            var task = await _context.EngineerTasks
                .Include(t => t.VendingMachine)
                .Include(t => t.AssignedTo)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                return NotFound($"Задача с ID {id} не найдена");

            var dto = _mapper.Map<EngineerTaskShortDto>(task);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении задачи {Id}", id);
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateEngineerTask([FromBody] CreateUpdateEngineerTaskDto dto)
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
            if (dto.AssignedToId.HasValue)
            {
                var employee = await _context.Employees.FindAsync(dto.AssignedToId.Value);
                if (employee == null)
                    return BadRequest("Сотрудник не найден");
            }

            var task = _mapper.Map<EngineerTask>(dto);
            task.Id = Guid.NewGuid();

            _context.EngineerTasks.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEngineerTask),
                new { id = task.Id },
                new { Id = task.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании задачи");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEngineerTask(Guid id, [FromBody] CreateUpdateEngineerTaskDto dto)
    {
        try
        {
            if (id == Guid.Empty)
                return BadRequest("Неверный идентификатор");

            var task = await _context.EngineerTasks.FindAsync(id);
            if (task == null)
                return NotFound($"Задача с ID {id} не найдена");

            // Проверка ТА
            var vendingMachine = await _context.VendingMachines.FindAsync(dto.VendingMachineId);
            if (vendingMachine == null)
                return BadRequest("Вендинговый аппарат не найден");

            // Проверка сотрудника
            if (dto.AssignedToId.HasValue && dto.AssignedToId.Value != task.AssignedToId)
            {
                var employee = await _context.Employees.FindAsync(dto.AssignedToId.Value);
                if (employee == null)
                    return BadRequest("Сотрудник не найден");
            }

            _mapper.Map(dto, task);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении задачи {Id}", id);
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    [HttpPatch("{id}/assign")]
    public async Task<IActionResult> AssignTask(Guid id, [FromBody] AssignEngineerTaskDto dto)
    {
        try
        {
            if (id == Guid.Empty)
                return BadRequest("Неверный идентификатор");

            var task = await _context.EngineerTasks.FindAsync(id);
            if (task == null)
                return NotFound($"Задача с ID {id} не найдена");

            var employee = await _context.Employees.FindAsync(dto.EmployeeId);
            if (employee == null)
                return BadRequest("Сотрудник не найден");

            _mapper.Map(dto, task);
            task.Status = TaskStatusEnum.Assigned;

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Задача назначена", EmployeeName = employee.FullName });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при назначении задачи {Id}", id);
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    [HttpPatch("{id}/complete")]
    public async Task<IActionResult> CompleteTask(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                return BadRequest("Неверный идентификатор");

            var task = await _context.EngineerTasks.FindAsync(id);
            if (task == null)
                return NotFound($"Задача с ID {id} не найдена");

            task.Status = TaskStatusEnum.Completed;
            task.CompletedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Задача завершена" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при завершении задачи {Id}", id);
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEngineerTask(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                return BadRequest("Неверный идентификатор");

            var task = await _context.EngineerTasks.FindAsync(id);
            if (task == null)
                return NotFound($"Задача с ID {id} не найдена");

            _context.EngineerTasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении задачи {Id}", id);
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }
}