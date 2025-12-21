using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using Database;
using Domain.Entities;
using Domain.Enums;
using DTOs.Core;
using DTOs.General;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CoreService.Controllers
{
    [Route("api/Core/[controller]")]
    [ApiController]
    public class VendingsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<VendingsController> _logger;

        public VendingsController(AppDbContext context, ILogger<VendingsController> logger)
        {
            _context = context;
            //_mapper = mapper;
            _logger = logger;
        }

        // GET: api/vendingmachines (список для таблицы)
        [HttpGet]
        public async Task<ActionResult> GetVendingMachines( CancellationToken cancellation,
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

                var totalCount = query.Count();

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
                    .ToListAsync(cancellation);

                //var result = _mapper.Map<List<VendingMachineShortDto>>(items);
                var result = items.Select(e => new VendingMachineShortDto(
                    Id: e.Id,
                    Name: e.Name,
                    Model: e.Model,
                    Location: e.Location,
                    SerialNumber: e.SerialNumber,
                    Status: e.Status.ToStringRu(),
                    CompanyName: e.Company?.Name ?? string.Empty, // или null
                    ModemNumber: e.Modem?.Imei ?? string.Empty,    // или null
                    NextMaintenanceDate: e.NextMaintenanceDate
                )).ToList();
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
                var dto = new VendingMachineDetailsDto()
                {
                    Id = machine.Id,
                    Name = machine.Name,
                    Location = machine.Location,
                    Address = machine.Address,
                    Model = machine.Model,
                    TotalRevenue = machine.TotalRevenue,
                    SerialNumber = machine.SerialNumber,
                    InventoryNumber = machine.InventoryNumber,
                    Manufacturer = machine.Manufacturer,
                    ManufactureDate = machine.ManufactureDate,
                    CommissioningDate = machine.CommissioningDate,
                    LastVerificationDate = machine.LastVerificationDate,
                    VerificationIntervalMonths = machine.VerificationIntervalMonths,
                    NextVerificationDate = machine.NextVerificationDate,
                    ResourceHours = machine.ResourceHours,
                    NextMaintenanceDate = machine.NextMaintenanceDate,
                    MaintenanceDurationHours = machine.MaintenanceDurationHours,
                    Status = machine.Status.ToStringRu(),
                    InventoryDate = machine.InventoryDate,
                    CreatedAt = machine.CreatedAt,

                    // Навигационные свойства с проверкой на null
                    CompanyName = machine.Company?.Name ?? string.Empty,
                    ModemImei = machine.Modem?.Imei ?? string.Empty,
                    ModemProvider = machine.Modem?.Provider ?? string.Empty,
                    CountryName = machine.ProducerCountry?.Name ?? string.Empty,
                    LastVerifiedBy = machine.LastVerificationEmployee?.FullName ?? string.Empty
                };
                //var dto = _mapper.Map<VendingMachineDetailsDto>(machine);
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
        //////////////////////////////////////////////////////////////////////////////
        ///
                // GET: api/vendingmachines/search?model=...&status=...&serialNumber=...
        [HttpGet("search")]
        public async Task<ActionResult> SearchVendingMachines(
            [FromQuery] string? model,
            [FromQuery] MachineStatusEnum? status,
            [FromQuery] string? serialNumber,
            [FromQuery] string? inventoryNumber,
            [FromQuery] string? manufacturer,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
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

                // Применяем фильтры
                if (!string.IsNullOrEmpty(model))
                    query = query.Where(v => v.Model != null && v.Model.Contains(model));

                if (status.HasValue)
                    query = query.Where(v => v.Status == status);

                if (!string.IsNullOrEmpty(serialNumber))
                    query = query.Where(v => v.SerialNumber != null && v.SerialNumber.Contains(serialNumber));

                if (!string.IsNullOrEmpty(inventoryNumber))
                    query = query.Where(v => v.InventoryNumber != null && v.InventoryNumber.Contains(inventoryNumber));

                if (!string.IsNullOrEmpty(manufacturer))
                    query = query.Where(v => v.Manufacturer != null && v.Manufacturer.Contains(manufacturer));

                

                if (fromDate.HasValue)
                    query = query.Where(v => v.CommissioningDate >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(v => v.CommissioningDate <= toDate.Value);

                var totalCount = await query.CountAsync();

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

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                if (page > totalPages) page = totalPages;

                var items = await query
                    .OrderBy(v => v.Name)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var result = _mapper.Map<List<VendingMachineShortDto>>(items);

                return Ok(new PagedResponse<VendingMachineShortDto>
                {
                    Items = result,
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при поиске вендинговых аппаратов");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        // POST: api/vendingmachines/import
        [HttpPost("import")]
        public async Task<ActionResult> ImportFromCsv(IFormFile file, CancellationToken cancellation)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("Файл не предоставлен");

                if (Path.GetExtension(file.FileName).ToLower() != ".csv")
                    return BadRequest("Поддерживаются только CSV файлы");

                var importedMachines = new List<VendingMachine>();
                var errors = new List<string>();
                var rowNumber = 0;

                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    HeaderValidated = null, // Отключаем проверку заголовков
                    MissingFieldFound = null, // Отключаем проверку отсутствующих полей
                    PrepareHeaderForMatch = args => args.Header.ToLower() // Приводим к нижнему регистру
                };

                using (var reader = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(reader, config))
                {
                    // Регистрируем маппинг
                    csv.Context.RegisterClassMap<VendingMachineCsvMap>();

                    // Читаем записи
                    var records = csv.GetRecordsAsync<VendingMachineCsvDto>(cancellation);
                    
                    await foreach (var record in records)
                    {
                        rowNumber++;

                        try
                        {
                            // Валидация записи
                            if (string.IsNullOrEmpty(record.SerialNumber))
                            {
                                errors.Add($"Строка {rowNumber}: Отсутствует серийный номер");
                                continue;
                            }

                            if (await _context.VendingMachines.AnyAsync(v => v.SerialNumber == record.SerialNumber))
                            {
                                errors.Add($"Строка {rowNumber}: Аппарат с серийным номером {record.SerialNumber} уже существует");
                                continue;
                            }

                            // Преобразование записи в сущность
                            //var machine = _mapper.Map<VendingMachine>(record);
                            var machine = new VendingMachine()
                            {
                                Name = record.Name,
                                Location = record.Location,
                                Address = record.Address,
                                InventoryNumber = record.InventoryNumber,
                                SerialNumber = record.SerialNumber,
                                Model = record.Model ?? "Не указана",
                                Manufacturer = record.Manufacturer ?? "Не указан",
                                ManufactureDate = record.ManufactureDate ?? record.ProductionDate ?? DateTime.UtcNow,
                                CommissioningDate = record.CommissioningDate ?? DateTime.UtcNow,
                                LastVerificationDate = record.LastVerificationDate,
                                InventoryDate = record.LastInventoryDate,

                                // ВАЖНО: Либо null, либо дата в БУДУЩЕМ
                                NextMaintenanceDate = record.NextMaintenanceDate.HasValue
                                ? (record.NextMaintenanceDate > DateTime.UtcNow
                                    ? record.NextMaintenanceDate
                                    : DateTime.UtcNow.AddDays(1)) // Если дата в прошлом - ставим завтра
                                : null, // Или просто null

                                ResourceHours = record.ResourceHours ?? 0,
                                MaintenanceDurationHours = record.MaintenanceDurationHours ?? 0,
                                VerificationIntervalMonths = record.VerificationIntervalMonths,
                                TotalRevenue = record.TotalRevenue ?? 0,
                                Status = ParseMachineStatus(record.Status),

                                // Системные поля - ВАЖНО: CreatedAt будет установлен базой
                                //Id = Guid.NewGuid(),
                                // CreatedAt = DateTime.UtcNow // НЕ УСТАНАВЛИВАЙТЕ ВРУЧНУЮ, пусть БД сама ставит
                            };


                            // Если нет даты изготовления, ставим текущую
                            if (!record.ProductionDate.HasValue)
                                machine.ManufactureDate = DateTime.UtcNow;

                            // Расчет даты следующей поверки
                            if (record.LastVerificationDate.HasValue && record.VerificationIntervalMonths.HasValue)
                            {
                                machine.NextVerificationDate = record.LastVerificationDate.Value
                                    .AddMonths(record.VerificationIntervalMonths.Value);
                            }

                            await _context.VendingMachines.AddAsync(machine, cancellation);
                            importedMachines.Add(machine);
                        }
                        catch (Exception ex)
                        {
                            errors.Add($"Строка {rowNumber}: Ошибка обработки - {ex.Message}");
                        }
                    }
                }

                // Сохранение успешно обработанных записей
                //if (importedMachines.Any())
                //{
                //    //await _context.VendingMachines.AddRangeAsync(importedMachines, cancellation);
                //    await _context.SaveChangesAsync(cancellation);
                //}
                await _context.SaveChangesAsync(cancellation);
                return Ok(new
                {
                    SuccessCount = importedMachines.Count,
                    ErrorCount = errors.Count,
                    Errors = errors,
                    Message = $"Импортировано {importedMachines.Count} аппаратов, ошибок: {errors.Count}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при импорте CSV файла");
                return StatusCode(500, "Ошибка импорта данных");
            }
        }

        // GET: api/vendingmachines/export
        [HttpGet("export")]
        public async Task<ActionResult> ExportToCsv([FromQuery] string? ids = null)
        {
            try
            {
                IQueryable<VendingMachine> query = _context.VendingMachines
                    .Include(v => v.Company)
                    .Include(v => v.ProducerCountry)
                    .Include(v => v.LastVerificationEmployee)
                    .AsNoTracking();

                // Фильтрация по IDs если переданы
                if (!string.IsNullOrEmpty(ids))
                {
                    var idList = ids.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(id => Guid.TryParse(id, out var guid) ? guid : Guid.Empty)
                        .Where(id => id != Guid.Empty)
                        .ToList();

                    if (idList.Any())
                    {
                        query = query.Where(v => idList.Contains(v.Id));
                    }
                }

                var machines = await query.ToListAsync();
                var records = _mapper.Map<List<VendingMachineExportDto>>(machines);

                // Генерация CSV
                using (var memoryStream = new MemoryStream())
                using (var writer = new StreamWriter(memoryStream))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(records);
                    writer.Flush();
                    memoryStream.Position = 0;

                    var fileName = $"vending-machines-export-{DateTime.Now:yyyyMMdd-HHmmss}.csv";
                    return File(memoryStream.ToArray(), "text/csv", fileName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при экспорте в CSV");
                return StatusCode(500, "Ошибка экспорта данных");
            }
        }

        // GET: api/vendingmachines/statistics
        [HttpGet("statistics")]
        public async Task<ActionResult> GetStatistics(CancellationToken cancellation)
        {
            try
            {
                var totalCount = await _context.VendingMachines.CountAsync(cancellation);
                var activeCount = await _context.VendingMachines.CountAsync(v => v.Status == MachineStatusEnum.Working, cancellation);
                var maintenanceCount = await _context.VendingMachines.CountAsync(v => v.Status == MachineStatusEnum.UnderMaintenance, cancellation);
                var inactiveCount = await _context.VendingMachines.CountAsync(v => v.Status == MachineStatusEnum.OutOfService, cancellation);

                var totalRevenue = await _context.VendingMachines.SumAsync(v => v.TotalRevenue, cancellation);
                var avgRevenue = totalCount > 0 ? totalRevenue / totalCount : 0;

                // Аппараты, требующие поверки (в течение месяца)
                var verificationDue = await _context.VendingMachines
                    .Where(v => v.NextVerificationDate.HasValue &&
                               v.NextVerificationDate <= DateTime.UtcNow.AddMonths(1))
                    .CountAsync(cancellation);

                // Аппараты, требующие обслуживания
                var maintenanceDue = await _context.VendingMachines
                    .Where(v => v.NextMaintenanceDate.HasValue &&
                               v.NextMaintenanceDate <= DateTime.UtcNow.AddDays(30))
                    .CountAsync(cancellation);

                // Распределение по производителям
                var manufacturerDistribution = await _context.VendingMachines
                    .GroupBy(v => v.Manufacturer)
                    .Select(g => new
                    {
                        Manufacturer = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .Take(10)
                    .ToListAsync(cancellation);

                return Ok(new
                {
                    Summary = new
                    {
                        TotalCount = totalCount,
                        ActiveCount = activeCount,
                        MaintenanceCount = maintenanceCount,
                        InactiveCount = inactiveCount
                    },
                    Revenue = new
                    {
                        Total = totalRevenue,
                        Average = avgRevenue
                    },
                    Upcoming = new
                    {
                        VerificationDue = verificationDue,
                        MaintenanceDue = maintenanceDue
                    },

                    TopManufacturers = manufacturerDistribution,
                    LastUpdated = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении статистики");
                return StatusCode(500, "Ошибка получения статистики");
            }
        }

        // GET: api/vendingmachines/check-serial/{serialNumber}
        [HttpGet("check-serial/{serialNumber}")]
        public async Task<ActionResult> CheckSerialNumber(string serialNumber, [FromQuery] Guid? excludeId = null)
        {
            try
            {
                if (string.IsNullOrEmpty(serialNumber))
                    return BadRequest("Серийный номер не указан");

                var query = _context.VendingMachines
                    .Where(v => v.SerialNumber == serialNumber);

                if (excludeId.HasValue)
                {
                    query = query.Where(v => v.Id != excludeId.Value);
                }

                var exists = await query.AnyAsync();

                return Ok(new
                {
                    SerialNumber = serialNumber,
                    Exists = exists,
                    Message = exists
                        ? "Серийный номер уже используется"
                        : "Серийный номер доступен"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при проверке серийного номера {SerialNumber}", serialNumber);
                return StatusCode(500, "Ошибка проверки серийного номера");
            }
        }

        // GET: api/vendingmachines/by-serial/{serialNumber}
        [HttpGet("by-serial/{serialNumber}")]
        public async Task<ActionResult> GetBySerialNumber(string serialNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(serialNumber))
                    return BadRequest("Серийный номер не указан");

                var machine = await _context.VendingMachines
                    .Include(v => v.Company)
                    .Include(v => v.Modem)
                    .Include(v => v.ProducerCountry)
                    .Include(v => v.LastVerificationEmployee)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(v => v.SerialNumber == serialNumber);

                if (machine == null)
                {
                    _logger.LogWarning("Аппарат с серийным номером {SerialNumber} не найден", serialNumber);
                    return NotFound(new { Message = $"Аппарат с серийным номером {serialNumber} не найден" });
                }

                var dto = _mapper.Map<VendingMachineDetailsDto>(machine);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при поиске аппарата по серийному номеру {SerialNumber}", serialNumber);
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        // GET: api/vendingmachines/due-for-verification
        [HttpGet("due-for-verification")]
        public async Task<ActionResult> GetMachinesDueForVerification(
            [FromQuery] int daysThreshold = 30,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var thresholdDate = DateTime.UtcNow.AddDays(daysThreshold);

                var query = _context.VendingMachines
                    .Include(v => v.Company)
                    .Where(v => v.NextVerificationDate.HasValue &&
                               v.NextVerificationDate <= thresholdDate &&
                               v.Status ==MachineStatusEnum.Working)
                    .OrderBy(v => v.NextVerificationDate)
                    .AsNoTracking();

                var totalCount = await query.CountAsync();

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

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                if (page > totalPages) page = totalPages;

                var items = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var result = _mapper.Map<List<VendingMachineShortDto>>(items);

                return Ok(new PagedResponse<VendingMachineShortDto>
                {
                    Items = result,
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении аппаратов, требующих поверки");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        // GET: api/vendingmachines/due-for-maintenance
        [HttpGet("due-for-maintenance")]
        public async Task<ActionResult> GetMachinesDueForMaintenance(
            [FromQuery] int daysThreshold = 30,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var thresholdDate = DateTime.UtcNow.AddDays(daysThreshold);

                var query = _context.VendingMachines
                    .Include(v => v.Company)
                    .Where(v => v.NextMaintenanceDate.HasValue &&
                               v.NextMaintenanceDate <= thresholdDate &&
                               v.Status ==MachineStatusEnum.Working)
                    .OrderBy(v => v.NextMaintenanceDate)
                    .AsNoTracking();

                var totalCount = await query.CountAsync();

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

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                if (page > totalPages) page = totalPages;

                var items = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var result = _mapper.Map<List<VendingMachineShortDto>>(items);

                return Ok(new PagedResponse<VendingMachineShortDto>
                {
                    Items = result,
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении аппаратов, требующих обслуживания");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        // PATCH: api/vendingmachines/bulk-status
        [HttpPatch("bulk-status")]
        public async Task<ActionResult> UpdateBulkStatus([FromBody] BulkStatusUpdateDto dto)
        {
            try
            {
                
                if (dto == null || dto.Ids == null || !dto.Ids.Any())
                    return BadRequest("Не указаны ID аппаратов");

                if (string.IsNullOrEmpty(dto.Status))
                    return BadRequest("Не указан новый статус");

                var validStatuses = new[] { "active", "maintenance", "inactive", "out_of_order" };
                if (!validStatuses.Contains(dto.Status))
                    return BadRequest($"Недопустимый статус. Допустимые значения: {string.Join(", ", validStatuses)}");

                var machines = await _context.VendingMachines
                    .Where(v => dto.Ids.Contains(v.Id))
                    .ToListAsync();

                if (!machines.Any())
                    return NotFound("Аппараты не найдены");

                foreach (var machine in machines)
                {
                    machine.Status = (MachineStatusEnum)int.Parse(dto.Status);
                    machine.LastVerificationDate = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Обновлен статус {Status} для {Count} аппаратов", dto.Status, machines.Count);
                return Ok(new
                {
                    UpdatedCount = machines.Count,
                    Message = $"Обновлен статус для {machines.Count} аппаратов"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при массовом обновлении статусов");
                return StatusCode(500, "Ошибка обновления статусов");
            }
        }
        private MachineStatusEnum ParseMachineStatus(string status)
        {
            return status.ToLower() switch
            {
                "работает" => MachineStatusEnum.Working,
                "исправен" => MachineStatusEnum.Working,
                "working" => MachineStatusEnum.Working,
                "active" => MachineStatusEnum.Working,
                "сломан" => MachineStatusEnum.Broken,
                "broken" => MachineStatusEnum.Broken,
                "неисправен" => MachineStatusEnum.Broken,
                "обслуживается" => MachineStatusEnum.UnderMaintenance,
                "на обслуживании" => MachineStatusEnum.UnderMaintenance,
                "maintenance" => MachineStatusEnum.UnderMaintenance,
                "underMaintenance" => MachineStatusEnum.UnderMaintenance,
                "выведен из строя" => MachineStatusEnum.OutOfService,
                "outOfService" => MachineStatusEnum.OutOfService
            };
        }
    }
    public class VendingMachineCsvRecord
    {
        public string? SerialNumber { get; set; }
        public string? InventoryNumber { get; set; }
        public string? Model { get; set; }
        public string? Manufacturer { get; set; }
        public string? Type { get; set; }
        public string? PaymentTypes { get; set; }
        public string? Location { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public DateTime? CommissioningDate { get; set; }
        public DateTime? LastVerificationDate { get; set; }
        public int? VerificationIntervalMonths { get; set; }
        public int? ResourceHours { get; set; }
        public int? CurrentHours { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
        public int? MaintenanceDurationHours { get; set; }
        public string? Status { get; set; }
        public string? StatusText { get; set; }
        public decimal? TotalRevenue { get; set; }
        public DateTime? LastInventoryDate { get; set; }
        public string? LastVerificationBy { get; set; }
        public string? Franchisee { get; set; }
    }

    public class VendingMachineCsvMap : ClassMap<VendingMachineCsvRecord>
    {
        public VendingMachineCsvMap()
        {
            Map(m => m.SerialNumber).Name("serialNumber");
            Map(m => m.InventoryNumber).Name("inventoryNumber");
            Map(m => m.Model).Name("model");
            Map(m => m.Manufacturer).Name("manufacturer");
            Map(m => m.Type).Name("type");
            Map(m => m.PaymentTypes).Name("paymentTypes");
            Map(m => m.Location).Name("location");
            Map(m => m.Address).Name("address");
            Map(m => m.Country).Name("country");
            Map(m => m.ManufactureDate).Name("productionDate").TypeConverterOption.Format("yyyy-MM-dd");
            Map(m => m.CommissioningDate).Name("commissioningDate").TypeConverterOption.Format("yyyy-MM-dd");
            Map(m => m.LastVerificationDate).Name("lastVerification").TypeConverterOption.Format("yyyy-MM-dd");
            Map(m => m.VerificationIntervalMonths).Name("verificationInterval");
            Map(m => m.ResourceHours).Name("resourceHours");
            Map(m => m.CurrentHours).Name("currentHours");
            Map(m => m.NextMaintenanceDate).Name("nextMaintenance").TypeConverterOption.Format("yyyy-MM-dd");
            Map(m => m.MaintenanceDurationHours).Name("maintenanceTime");
            Map(m => m.Status).Name("status");
            Map(m => m.StatusText).Name("statusText");
            Map(m => m.TotalRevenue).Name("totalRevenue");
            Map(m => m.LastInventoryDate).Name("lastInventory").TypeConverterOption.Format("yyyy-MM-dd");
            Map(m => m.LastVerificationBy).Name("lastVerificationBy");
            Map(m => m.Franchisee).Name("franchisee");
        }
    }
}
