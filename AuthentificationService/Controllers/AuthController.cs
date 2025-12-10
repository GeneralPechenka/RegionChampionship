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
        private readonly ITokenHeaderService _tokenHeaderService;
        private readonly IAuthService _authService;

        
        public AuthController(IAuthService authService, ITokenHeaderService tokenHeaderService)
        {
            _tokenHeaderService = tokenHeaderService;
            _authService = authService;
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
        public async Task<IActionResult> Logout(CancellationToken token)
        {
            try
            {
                var jwtToken = _tokenHeaderService.GetTokenFromRequest(HttpContext.Request);
                await _authService.LogoutAsync(jwtToken, token);
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
        public async Task<IActionResult> RefreshToken(CancellationToken token)
        {
            try
            {
                var jwtToken = _tokenHeaderService.GetTokenFromRequest(HttpContext.Request);
                if (string.IsNullOrWhiteSpace(jwtToken))
                {
                    return BadRequest("Некорректный формат доступа. Возможно, вы не авторизованы.");
                }
                var newToken = await _authService.RefreshTokenAsync(jwtToken, token);

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
