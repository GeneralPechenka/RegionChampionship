using Domain.Entities;

namespace Interfaces
{
    public interface ITokenProvider
    {
        Task<string> GenerateTokenAsync(Employee user);
        Task<Guid> GetIdFromJwtTokenAsync(string jwtToken);
        Task<bool> CheckExpireJwtTokenAsync(string jwtToken);
    }
}
