using AuthentificationService.Interfaces;
using Database;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using DTOs.Authentification;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Validators;

namespace AuthentificationService.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;
        private readonly IPasswordHasher _hasher;
        private readonly ITokenProvider _tokenProvider;
        private readonly UserValidator _validator;
        private readonly TokenStorage _tokenStorage;


        public AuthService(IConfiguration config, AppDbContext context, IPasswordHasher hasher, 
            ITokenProvider tokenProvider, UserValidator validator, TokenStorage tokenStorage)
        {
            _config = config;
            _context = context;
            _hasher = hasher;
            _tokenProvider = tokenProvider;
            _validator = validator;
            _tokenStorage = tokenStorage;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken token)
        {
            //Провалидировать данные
            ValidateLoginRequest(request);

            //Найти сотрудника по email
            var user = await _context.Employees.FirstOrDefaultAsync(e => e.Email == request.Email, token);
            //Проверить пароль
            var verified = _hasher.VerifyPassword(request.PasswordHash, user.PasswordHash);

            // Назначить токен (присвоить, сгенерировать или обновить)

            var exist = _tokenStorage.ValidTokens.TryGetValue(request.Email, out var jwtToken);
            if (exist)
            {
                //TODO:Проверить, действителен ли токен
                var valid = await _tokenProvider.CheckExpireJwtTokenAsync(jwtToken);
                if (!valid)
                {
                    _tokenStorage.RevokedTokens.Add(jwtToken);
                    _tokenStorage.ValidTokens.Remove(request.Email);
                    jwtToken = await _tokenProvider.GenerateTokenAsync(user);
                    _tokenStorage.ValidTokens[user.Email] = jwtToken;
                }

            }
            else
            {
                jwtToken = await _tokenProvider.GenerateTokenAsync(user);
                _tokenStorage.ValidTokens[user.Email] = jwtToken;
            }
            return new LoginResponseDto(
                Token: jwtToken,
                Expires: DateTime.UtcNow.AddMinutes(30),
                UserInfo: new UserInfoDto(user.Email, user.FullName, user.Role.ToStringRu())
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="EmployeeException"></exception>
        public async Task<LoginResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken token)
        {
            //Провалидировать данные
            ValidateRegisterRequest(request);
            //Проверить, есть ли такой пользователь в БД
            var exists = await _context.Employees.AnyAsync(e => e.Email == request.Email, token);
            //Добавить в БД
            if (exists)
                throw new EmployeeException("Пользователь с таким Email уже существует");

            var user = new Employee
            {
                Email = request.Email,
                FullName = request.Fullname,
                PasswordHash = _hasher.Encrypt(request.PasswordHash),
                Role = request.Role
            };
            await _context.Employees.AddAsync(user, token);
            await _context.SaveChangesAsync(token);
            //Сгенерировать токен
            var jwt = await _tokenProvider.GenerateTokenAsync(user);
            //Вернуть DTO
            return new LoginResponseDto(
                Token: jwt,
                Expires: DateTime.UtcNow.AddMinutes(30),
                UserInfo: new UserInfoDto(user.Email, user.FullName, user.Role.ToStringRu())
            );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="EmployeeException"></exception>
        public async Task<RefreshTokenDto> RefreshTokenAsync(string token, CancellationToken cancellation)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException("token", "не может быть пустым");
            }
            var isExpired = await _tokenProvider.CheckExpireJwtTokenAsync(token);
            if (isExpired)
            {
                return new RefreshTokenDto(token);
            }
            var userId = await _tokenProvider.GetIdFromJwtTokenAsync(token);
            var user = await _context.Employees.FirstOrDefaultAsync(e => e.Id == userId, cancellation)
                ?? throw new EmployeeException("Такого пользователя не существует");
            var dto = new RefreshTokenDto(await _tokenProvider.GenerateTokenAsync(user));
            _tokenStorage.ValidTokens.Remove(user.Email);
            _tokenStorage.ValidTokens.Add(user.Email, dto.Token);
            _tokenStorage.RevokedTokens.Add(token);

            return dto;
        }
        public async Task LogoutAsync(string token, CancellationToken cancellation)
        {
            if(string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException("token","не должен быть пустым");
            }
            var email = _tokenStorage.ValidTokens.FirstOrDefault(e=>e.Value == token).Key;
            if(_tokenStorage.RevokedTokens.Contains(token) || string.IsNullOrEmpty(email))
            {
                throw new AuthException("Ошибка аутентификации: пользователь не вошёл в систему");
            }
            _tokenStorage.RevokedTokens.Add(token);
            _tokenStorage.ValidTokens.Remove(email);
        }

        private void ValidateLoginRequest(LoginRequestDto request)
        {
            if (!_validator.ValidateEmail(request.Email))
            {
                throw new ArgumentException("Email не соответствует принятому формату");
            }
            if (!_validator.ValidatePassword(request.PasswordHash))
            {
                throw new ArgumentException("Пароль не соответствует принятому формату");
            }
        }

        private void ValidateRegisterRequest(RegisterRequestDto request)
        {
            if (!_validator.ValidateEmail(request.Email))
            {
                throw new ArgumentException("Email не соответствует принятому формату");
            }
            if (!_validator.ValidatePassword(request.PasswordHash))
            {
                throw new ArgumentException("Пароль не соответствует принятому формату");
            }
            if (!_validator.ValidateFullname(request.Fullname))
            {
                throw new ArgumentException("ФИО не соответствует принятому формату");
            }
            
        }
    }
}
