using Database;
using Domain.Entities;
using Domain.Enums;
using DTOs.Core.Employeee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreService.Controllers
{
    [Route("api/Core/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;
      

        public EmployeesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Franchisor")]
        public async Task<ActionResult<EmployeesListResponseDto>> GetEmployees(
            [FromBody] SearchEmployeesRequestDto request)
        {
            try
            {
                var query = _context.Employees
                    .Include(e => e.Company)
                    .Include(e => e.VerifiedMachines)
                    .Include(e => e.Maintenances)
                    .AsNoTracking()
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(request.FullName))
                {
                    query = query.Where(e => e.FullName.Contains(request.FullName));
                }

                if (!string.IsNullOrWhiteSpace(request.Email))
                {
                    query = query.Where(e => e.Email.Contains(request.Email));
                }

                if (request.Role.HasValue)
                {
                    query = query.Where(e => e.Role == request.Role.Value);
                }

                if (request.CompanyId.HasValue)
                {
                    query = query.Where(e => e.CompanyId == request.CompanyId.Value);
                }

                if (request.CreatedFrom.HasValue)
                {
                    query = query.Where(e => e.CreatedAt >= request.CreatedFrom.Value);
                }

                if (request.CreatedTo.HasValue)
                {
                    query = query.Where(e => e.CreatedAt <= request.CreatedTo.Value);
                }

                if (request.HasVerifiedMachines.HasValue)
                {
                    query = request.HasVerifiedMachines.Value
                        ? query.Where(e => e.VerifiedMachines.Any())
                        : query.Where(e => !e.VerifiedMachines.Any());
                }

                if (request.HasMaintenances.HasValue)
                {
                    query = request.HasMaintenances.Value
                        ? query.Where(e => e.Maintenances.Any())
                        : query.Where(e => !e.Maintenances.Any());
                }

                query = request.SortBy?.ToLower() switch
                {
                    "email" => request.SortDescending
                        ? query.OrderByDescending(e => e.Email)
                        : query.OrderBy(e => e.Email),
                    "role" => request.SortDescending
                        ? query.OrderByDescending(e => e.Role)
                        : query.OrderBy(e => e.Role),
                    "createdat" => request.SortDescending
                        ? query.OrderByDescending(e => e.CreatedAt)
                        : query.OrderBy(e => e.CreatedAt),
                    _ => request.SortDescending
                        ? query.OrderByDescending(e => e.FullName)
                        : query.OrderBy(e => e.FullName)
                };

                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

                var employees = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                var employeeResponses = employees.Select(e => new EmployeeResponseDto(
                    Id: e.Id,
                    FullName: e.FullName,
                    Email: e.Email,
                    Role: e.Role,
                    CreatedAt: e.CreatedAt,
                    CompanyId: e.CompanyId,
                    CompanyName: e.Company?.Name,
                    VerifiedMachinesCount: e.VerifiedMachines?.Count ?? 0,
                    MaintenancesCount: e.Maintenances?.Count ?? 0
                )).ToList();

                var response = new EmployeesListResponseDto(
                    Employees: employeeResponses,
                    TotalCount: totalCount,
                    PageNumber: request.PageNumber,
                    PageSize: request.PageSize,
                    TotalPages: totalPages
                );

                return Ok(response);
            }
            catch (Exception ex)
            {
                
                return StatusCode(StatusCodes.Status500InternalServerError, "Внутренняя ошибка сервера");
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeResponseDto>> GetEmployee(Guid id)
        {
            try
            {
                var employee = await _context.Employees
                    .Include(e => e.Company)
                    .Include(e => e.VerifiedMachines)
                    .Include(e => e.Maintenances)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (employee == null)
                {
                    return NotFound($"Сотрудник с ID {id} не найден");
                }

                var response = new EmployeeResponseDto(
                    Id: employee.Id,
                    FullName: employee.FullName,
                    Email: employee.Email,
                    Role: employee.Role,
                    CreatedAt: employee.CreatedAt,
                    CompanyId: employee.CompanyId,
                    CompanyName: employee.Company?.Name,
                    VerifiedMachinesCount: employee.VerifiedMachines?.Count ?? 0,
                    MaintenancesCount: employee.Maintenances?.Count ?? 0
                );

                return Ok(response);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpGet("company/{companyId}")]
        public async Task<ActionResult<List<EmployeeSimpleResponseDto>>> GetEmployeesByCompany(Guid companyId)
        {
            try
            {
                var employees = await _context.Employees
                            .Include(e => e.Company)
                            .Where(e => e.CompanyId == companyId)
                            .OrderBy(e => e.FullName)
                            .Select(e => new EmployeeSimpleResponseDto(
                                e.Id,
                                e.FullName,
                                e.Email,
                                e.Role,
                                e.Company != null ? e.Company.Name : null
                            ))
                            .ToListAsync();

                return Ok(employees);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<EmployeeResponseDto>> CreateEmployee([FromBody] EmployeeRequestDto request)
        {
            try
            {
                // Проверка уникальности email
                var existingEmployee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.Email == request.Email);

                if (existingEmployee != null)
                {
                    return Conflict("Сотрудник с таким email уже существует");
                }

                // Проверка компании, если указана
                if (request.CompanyId.HasValue)
                {
                    var companyExists = await _context.Companies
                        .AnyAsync(c => c.Id == request.CompanyId.Value);

                    if (!companyExists)
                    {
                        return BadRequest( "Указанная компания не существует");
                    }
                }

                var employee = new Employee
                {
                    Id = Guid.NewGuid(),
                    FullName = request.FullName,
                    Email = request.Email,
                    Role = request.Role,
                    CompanyId = request.CompanyId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                // Получаем созданного сотрудника с включенными зависимостями
                var company = await _context.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.CompanyId);
                var createdEmployee = await _context.Employees
                    .Include(e => e.Company)
                    .FirstOrDefaultAsync(e => e.Id == employee.Id);

                var response = new EmployeeResponseDto(
                    Id: employee.Id,
                    FullName: employee.FullName,
                    Email: employee.Email,
                    Role: employee.Role,
                    CreatedAt: employee.CreatedAt,
                    CompanyId: employee.CompanyId,
                    CompanyName: company.Name,
                    VerifiedMachinesCount: 0,
                    MaintenancesCount: 0
                );

                return CreatedAtAction(nameof(GetEmployee), response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<EmployeeResponseDto>> UpdateEmployee(
            Guid id,
            [FromBody] EmployeeRequestDto request)
        {
            try
            {
                var employee = await _context.Employees
                    .Include(e => e.Company)
                    .Include(e => e.VerifiedMachines)
                    .Include(e => e.Maintenances)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (employee == null)
                {
                    return NotFound($"Сотрудник с ID {id} не найден");
                }

                // Обновление полей
                employee.FullName = request.FullName;

                if (!string.IsNullOrEmpty(request.Email) && request.Email != employee.Email)
                {

                    var emailExists = await _context.Employees
                        .AnyAsync(e => e.Email == request.Email && e.Id != id);

                    if (emailExists)
                    {
                        return Conflict("Сотрудник с таким email уже существует");
                    }

                    employee.Email = request.Email;
                }

                 employee.Role = request.Role;
                

                if (request.CompanyId.HasValue)
                {
                    // Проверка существования компании
                    var companyExists = await _context.Companies
                        .AnyAsync(c => c.Id == request.CompanyId.Value);

                    if (!companyExists)
                    {
                        return BadRequest("Указанная компания не существует" );
                    }

                    employee.CompanyId = request.CompanyId.Value;
                }

                await _context.SaveChangesAsync();

                var response = new EmployeeResponseDto(
                    Id: employee.Id,
                    FullName: employee.FullName,
                    Email: employee.Email,
                    Role: employee.Role,
                    CreatedAt: employee.CreatedAt,
                    CompanyId: employee.CompanyId,
                    CompanyName: employee.Company?.Name,
                    VerifiedMachinesCount: employee.VerifiedMachines?.Count ?? 0,
                    MaintenancesCount: employee.Maintenances?.Count ?? 0
                );

                return Ok(response);
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpDelete("{id}")]
         [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEmployee(Guid id)
        {
            try
            {
                var result = await _context.Employees
                    .Where(e => e.Id == id)
                    .ExecuteDeleteAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpGet("roles")]
        public IActionResult GetEmployeeRoles()
        {
            var roles = Enum.GetValues(typeof(EmployeeRoleEnum))
                .Cast<EmployeeRoleEnum>()
                .Select(r => new
                {
                    Id = (int)r,
                    Name = r.ToString(),
                    DisplayName = r.ToStringRu()
                })
                .ToList();

            return Ok(roles);
        }

        //#region Вспомогательные методы

        //private string GetRoleDisplayName(EmployeeRoleEnum role)
        //{
        //    return role switch
        //    {
        //        EmployeeRoleEnum.Admin => "Администратор",
        //        EmployeeRoleEnum.Manager => "Менеджер",
        //        EmployeeRoleEnum.Franchisee => "Франчайзер",
        //        EmployeeRoleEnum.Technician => "Техник",
        //        EmployeeRoleEnum.Operator => "Оператор",
        //        EmployeeRoleEnum.Employee => "Сотрудник",
        //        _ => role.ToString()
        //    };
        //}

        //#endregion
    }
}
