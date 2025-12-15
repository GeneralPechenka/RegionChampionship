using AutoMapper;
using Database;
using Domain.Entities;
using DTOs.Core;
using DTOs.General;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreService.Controllers
{
    [Route("api/Core/[controller]")]
    [ApiController]
    public class VendingsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<VendingsController> _logger;

        public VendingsController(AppDbContext context, IMapper mapper, ILogger<VendingsController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/vendingmachines (список для таблицы)
        [HttpGet]
        public async Task<ActionResult> GetVendingMachines(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                // Валидация параметров
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var query = _context.VendingMachines
                    .Include(v => v.Company)
                    .Include(v => v.Modem)
                    .AsNoTracking();

                var totalCount = await query.CountAsync();

                // Проверка на пустой результат
                if (totalCount == 0)
                {
                    return Ok(new PagedResponse<VendingMachineShortDto>
                    {
                        Items = new List<VendingMachineShortDto>(),
                        Page = page,
                        PageSize = pageSize,
                        TotalCount = 0
                    });
                }

                // Проверка на выход за границы
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                if (page > totalPages) page = totalPages;

                var items = await query
                    .OrderBy(v => v.Name)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var result = _mapper.Map<List<VendingMachineShortDto>>(items);

                var response = new PagedResponse<VendingMachineShortDto>
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
                _logger.LogError(ex, "Ошибка при получении списка вендинговых аппаратов");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        // GET: api/vendingmachines/{id} (детали для формы редактирования)
        [HttpGet("{id}")]
        public async Task<ActionResult> GetVendingMachine(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest("Неверный идентификатор");

                var machine = await _context.VendingMachines
                    .Include(v => v.Company)
                    .Include(v => v.Modem)
                    .Include(v => v.ProducerCountry)
                    .Include(v => v.LastVerificationEmployee)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (machine == null)
                {
                    _logger.LogWarning("Вендинговый аппарат с ID {Id} не найден", id);
                    return NotFound(new { Message = $"Вендинговый аппарат с ID {id} не найден" });
                }

                var dto = _mapper.Map<VendingMachineDetailsDto>(machine);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении вендингового аппарата с ID {Id}", id);
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        // POST: api/vendingmachines
        [HttpPost]
        public async Task<ActionResult> CreateVendingMachine(
            [FromBody] CreateUpdateVendingMachineDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Данные не предоставлены");

                // Проверки уникальности
                if (await _context.VendingMachines.AnyAsync(v => v.SerialNumber == dto.SerialNumber))
                {
                    _logger.LogWarning("Попытка создания ТА с существующим серийным номером: {SerialNumber}", dto.SerialNumber);
                    return BadRequest("ТА с таким серийным номером уже существует");
                }

                if (dto.CommissioningDate < dto.ManufactureDate)
                    return BadRequest("Дата ввода в эксплуатацию не может быть раньше даты изготовления");

                if (dto.ResourceHours <= 0)
                    return BadRequest("Ресурс ТА должен быть положительным числом");

                if (dto.MaintenanceDurationHours < 1 || dto.MaintenanceDurationHours > 20)
                    return BadRequest("Время обслуживания должно быть от 1 до 20 часов");

                var machine = _mapper.Map<VendingMachine>(dto);
                machine.Id = Guid.NewGuid();
                machine.CreatedAt = DateTime.UtcNow;

                // Расчет даты следующей поверки
                if (dto.LastVerificationDate.HasValue && dto.VerificationIntervalMonths.HasValue)
                {
                    machine.NextVerificationDate = dto.LastVerificationDate.Value
                        .AddMonths(dto.VerificationIntervalMonths.Value);
                }

                _context.VendingMachines.Add(machine);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Создан новый вендинговый аппарат с ID {Id}", machine.Id);
                return CreatedAtAction(nameof(GetVendingMachine),
                    new { id = machine.Id },
                    new { Id = machine.Id, Message = "Вендинговый аппарат успешно создан" });
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Ошибка базы данных при создании вендингового аппарата");
                return StatusCode(500, "Ошибка сохранения данных");
            }
            catch (AutoMapperMappingException mapEx)
            {
                _logger.LogError(mapEx, "Ошибка маппинга данных при создании вендингового аппарата");
                return BadRequest("Ошибка преобразования данных");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании вендингового аппарата");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        // PUT: api/vendingmachines/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVendingMachine(
            Guid id,
            [FromBody] CreateUpdateVendingMachineDto dto)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest("Неверный идентификатор");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var machine = await _context.VendingMachines.FindAsync(id);
                if (machine == null)
                {
                    _logger.LogWarning("Попытка обновления несуществующего вендингового аппарата с ID {Id}", id);
                    return NotFound(new { Message = $"Вендинговый аппарат с ID {id} не найден" });
                }

                // Проверки уникальности (исключая текущий)
                if (await _context.VendingMachines
                    .AnyAsync(v => v.Id != id && v.SerialNumber == dto.SerialNumber))
                {
                    _logger.LogWarning("Попытка обновления ТА на существующий серийный номер: {SerialNumber}", dto.SerialNumber);
                    return BadRequest("ТА с таким серийным номером уже существует");
                }

                // Валидация бизнес-правил
                if (dto.CommissioningDate < dto.ManufactureDate)
                    return BadRequest("Дата ввода в эксплуатацию не может быть раньше даты изготовления");

                if (dto.ResourceHours <= 0)
                    return BadRequest("Ресурс ТА должен быть положительным числом");

                if (dto.MaintenanceDurationHours < 1 || dto.MaintenanceDurationHours > 20)
                    return BadRequest("Время обслуживания должно быть от 1 до 20 часов");

                // Сохраняем неизменяемые поля
                var createdAt = machine.CreatedAt;
                var totalRevenue = machine.TotalRevenue;

                _mapper.Map(dto, machine);

                // Восстанавливаем неизменяемые поля
                machine.CreatedAt = createdAt;
                machine.TotalRevenue = totalRevenue;

                // Пересчет даты следующей поверки
                if (dto.LastVerificationDate.HasValue && dto.VerificationIntervalMonths.HasValue)
                {
                    machine.NextVerificationDate = dto.LastVerificationDate.Value
                        .AddMonths(dto.VerificationIntervalMonths.Value);
                }
                else
                {
                    machine.NextVerificationDate = null;
                }

                _context.VendingMachines.Update(machine);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Обновлен вендинговый аппарат с ID {Id}", id);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogWarning("Конфликт параллельного доступа при обновлении вендингового аппарата с ID {Id}", id);
                return Conflict("Данные были изменены другим пользователем. Пожалуйста, обновите страницу и попробуйте снова.");
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Ошибка базы данных при обновлении вендингового аппарата с ID {Id}", id);
                return StatusCode(500, "Ошибка сохранения данных");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении вендингового аппарата с ID {Id}", id);
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        // DELETE: api/vendingmachines/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendingMachine(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest("Неверный идентификатор");

                var machine = await _context.VendingMachines
                    .Include(v => v.Sales)
                    .Include(v => v.Maintenances)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (machine == null)
                {
                    _logger.LogWarning("Попытка удаления несуществующего вендингового аппарата с ID {Id}", id);
                    return NotFound();
                }

                // Проверка на связанные данные (опционально)
                if (machine.Sales?.Any() == true)
                    return BadRequest("Невозможно удалить аппарат, так как существуют связанные продажи");

                _context.VendingMachines.Remove(machine);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Удален вендинговый аппарат с ID {Id}", id);
                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Ошибка базы данных при удалении вендингового аппарата с ID {Id}", id);
                return StatusCode(500, "Ошибка удаления данных");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении вендингового аппарата с ID {Id}", id);
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }
    }
}
