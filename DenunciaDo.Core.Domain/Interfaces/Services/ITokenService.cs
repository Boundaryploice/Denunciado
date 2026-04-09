using DenunciaDo.Core.Domain.Entities;
using System.Security.Claims;

namespace DenunciaDo.Core.Application.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(User user, IList<string> roles);
        ClaimsPrincipal ValidateToken(string token);
    }
}
