using AutoMapper;
using Database;
using Domain.Entities;
using DTOs.Core.PayType;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayTypesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<PayTypesController> _logger;

        public PayTypesController(AppDbContext context, IMapper mapper, ILogger<PayTypesController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetPayAllTypes(CancellationToken cancellation)
        {
            try
            {
                var payTypes = await _context.PayTypes
                    .AsNoTracking()
                    .ToListAsync(cancellation);
                var dto = _mapper.Map<List<PayTypeShortDto>>(payTypes);
                return Ok(dto);

            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Ошибка при получении списка задач");
                return StatusCode(StatusCodes.Status500InternalServerError,"Произошла внутренняя ошибка сервера");
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPayTypeByVendingId([FromQuery] Guid vendingId, CancellationToken cancellation)
        {
            try
            {
                var result = await _context.VendingAndPayTypes
                    .AsNoTracking()
                    .Include(p => p.PayType)
                    .Where(p => p.VendingMachineId == vendingId)
                    .ToListAsync(cancellation);
                var dto = _mapper.Map<List<PayTypeShortDto>>(result);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка задач");
                return StatusCode(StatusCodes.Status500InternalServerError, "Произошла внутренняя ошибка сервера");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromQuery] CreateUpdatePayTypeDto request, CancellationToken cancellation)
        {
            try
            {
                if (await _context.PayTypes.AsNoTracking().AnyAsync(p => p.Name.ToLower()==request.Name.ToLower(), cancellation))
                {
                    return BadRequest("Такой способ оплаты уже существует");
                }
                var payType = _mapper.Map<PayType>(request);
                await _context.PayTypes.AddAsync(payType,cancellation);
                await _context.SaveChangesAsync(cancellation);
                return Created();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка задач");
                return StatusCode(StatusCodes.Status500InternalServerError, "Произошла внутренняя ошибка сервера");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromQuery] CreateUpdatePayTypeDto request, CancellationToken cancellation)
        {
            try
            {
                await _context.PayTypes
                    .Where(p => p.Name.ToLower() == request.Name.ToLower())
                    .ExecuteUpdateAsync(s => s.SetProperty(t => t.Name, request.Name)
                    .SetProperty(t => t.Code, request.Code), cancellation);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка задач");
                return StatusCode(StatusCodes.Status500InternalServerError, "Произошла внутренняя ошибка сервера");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] CreateUpdatePayTypeDto request, CancellationToken cancellation)
        {
            try
            {
                await _context.PayTypes
                    .Where(p => p.Name.ToLower() == request.Name.ToLower())
                    .ExecuteDeleteAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка задач");
                return StatusCode(StatusCodes.Status500InternalServerError, "Произошла внутренняя ошибка сервера");
            }
        }
    }
}
