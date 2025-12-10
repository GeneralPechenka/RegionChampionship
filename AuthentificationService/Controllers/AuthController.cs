using AuthentificationService.Interfaces;
using Database;
using Domain.Exceptions;
using DTOs.Authentification;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace AuthentificationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IPasswordHasher _hasher;

        public AuthController(IAuthService authService, IPasswordHasher hasher)
        {
            _authService = authService;
            _hasher = hasher;
        }
        [HttpPost("test")]
        public async Task<IActionResult> Test()
        {

            return Ok("test complited");
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken token)
        {
            try
            {
                var dto= await _authService.LoginAsync(request, token);
                return Ok(dto);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto request, CancellationToken token)
        {
            try
            {
                await _authService.LogoutAsync(request.Token, token);
                return Ok();
            }
            catch(AuthException ex)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, ex.Message);
            }
            catch(ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request, CancellationToken token)
        {
            try
            {
                var dto = await _authService.RegisterAsync(request, token);

                return Ok(dto);
            }
            catch (EmployeeException ex)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost("Refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto request, CancellationToken token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Token))
                {
                    return BadRequest("Некорректный формат доступа. Возможно, вы не авторизованы.");
                }
                var newToken = await _authService.RefreshTokenAsync(request.Token, token);

                return Ok(newToken);
            }
            catch(EmployeeException ex)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, ex.Message);
            }
            catch(ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }

}
