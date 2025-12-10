using Database;
using Domain.Entities;
using Domain.Exceptions;
using DTOs.Core.Company;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreService.Controllers
{
    [Route("api/Core/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public CompaniesController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpPost("all")]
        public async Task<IActionResult> GetAll([FromBody] SearchCompaniesRequestDto requestDto,CancellationToken token)
        {
            try
            {
                var query = _dbContext.Companies
                    .Include(c => c.VendingMachines)
                    .Include(c => c.Employees)
                    .AsNoTracking()
                    .AsQueryable();

                // КОРРЕКТНАЯ фильтрация (обратите внимание - WHERE, а не Where!)
                if (!string.IsNullOrWhiteSpace(requestDto.Email))
                {
                    query = query.Where(c => c.Email.Contains(requestDto.Email));
                }

                if (!string.IsNullOrWhiteSpace(requestDto.Name))
                {
                    query = query.Where(c => c.Name.Contains(requestDto.Name));
                }

                if (!string.IsNullOrWhiteSpace(requestDto.Phone))
                {
                    query = query.Where(c => c.Phone.Contains(requestDto.Phone));
                }

                if (!string.IsNullOrWhiteSpace(requestDto.AddressContains))
                {
                    query = query.Where(c =>
                        c.Address != null &&
                        c.Address.Contains(requestDto.AddressContains));
                }

                if (requestDto.CreatedFrom.HasValue)
                {
                    query = query.Where(c => c.CreatedAt >= requestDto.CreatedFrom.Value);
                }

                if (requestDto.CreatedTo.HasValue)
                {
                    query = query.Where(c => c.CreatedAt <= requestDto.CreatedTo.Value);
                }

                if (requestDto.HasMachines.HasValue)
                {
                    query = requestDto.HasMachines.Value
                        ? query.Where(c => c.VendingMachines.Any())
                        : query.Where(c => !c.VendingMachines.Any());
                }

                if (requestDto.HasEmployees.HasValue)
                {
                    query = requestDto.HasEmployees.Value
                        ? query.Where(c => c.Employees.Any())
                        : query.Where(c => !c.Employees.Any());
                }

                // Пагинация
                var totalCount = await query.CountAsync();

                query = query
                    .Skip((requestDto.PageNumber - 1) * requestDto.PageSize)
                    .Take(requestDto.PageSize);

                var result = await query.ToListAsync(token);

                var companiesDto = result.Select(company => new CompanyResponseDto(
                    Id: company.Id,
                    Name: company.Name,
                    Address: company.Address,
                    Phone: company.Phone,
                    Email: company.Email,
                    CreatedAt: company.CreatedAt,
                    TotalMachines: company.VendingMachines?.Count ?? 0,
                    TotalEmployees: company.Employees?.Count ?? 0
                   
                )).ToList();

                var response = new CompaniesListResponseDto(
                    Companies: companiesDto,
                    TotalCount: totalCount,
                    PageNumber: requestDto.PageNumber,
                    PageSize: requestDto.PageSize,
                    TotalPages: (int)Math.Ceiling(totalCount / (double)requestDto.PageSize)
                );

                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, 
                    "Произошла внутренняя ошибка сервера. Попробуйте позже.");
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompanyById(Guid id, CancellationToken token)
        {
            try
            {
                if(id==Guid.Empty)
                {
                    return BadRequest("Идентификатор компании не может быть пусты");
                }
                var company = await _dbContext.Companies
                    .AsNoTracking()
                    .Include(c=>c.VendingMachines)
                    .Include(c=>c.Employees)
                    .FirstOrDefaultAsync(c => c.Id == id, token)
                    ?? throw new CompanyException("Компания не найдена.");

                return Ok(new CompanyResponseDto(
                    id, 
                    company.Name,
                    company.Address,
                    company.Phone, 
                    company.Email, 
                    company.CreatedAt,
                    company.VendingMachines.Count,
                    company.Employees.Count
                    ));
            }
            catch(CompanyException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception) 
            {

                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Произошла внутренняя ошибка сервера. Попробуйте позже.");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CompanyRequestDto request,CancellationToken token)
        {
            try
            {
                //TODO: добавить валидацию
                var exist = await _dbContext.Companies.AsNoTracking().AnyAsync(c => c.Name == request.Name, token);
                if (exist)
                {
                    return StatusCode(StatusCodes.Status409Conflict, "Такая компания уже существует");
                }
                var company = new Company
                {
                    Name = request.Name,
                    Email = request.Email,
                    Address = request.Address,
                    CreatedAt = DateTime.UtcNow,
                    Phone = request.Phone
                };
                await _dbContext.Companies.AddAsync(company, token);
                await _dbContext.SaveChangesAsync(token);
                return Ok($"Компания {request.Name} успешно создана");
            }
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Произошла внутренняя ошибка сервера. Попробуйте позже.");
            }
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] CompanyRequestDto request, CancellationToken cancellation)
        {
            try
            {
                //TODO: добавить валидацию
                var result = await _dbContext.Companies.Where(c=>c.Email==request.Email).ExecuteDeleteAsync(cancellation);
                return Ok("Компания удалена");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Произошла внутренняя ошибка сервера. Попробуйте позже.");
            }
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CompanyRequestDto request, CancellationToken cancellation)
        {
            try
            {
                //TODO: добавить валидацию
                var result = await _dbContext.Companies
                    .Where(c => c.Email == request.Email)
                    .ExecuteUpdateAsync(
                        t => t.SetProperty(c => c.Address, request.Address)
                        .SetProperty(c => c.Name, request.Name)
                        .SetProperty(c => c.Phone, request.Phone), 
                        cancellation
                    );
                return Ok("Компания изменена");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Произошла внутренняя ошибка сервера. Попробуйте позже.");
            }
        }


    }
}
