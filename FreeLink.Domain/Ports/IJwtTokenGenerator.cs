using FreeLink.Domain.Entities;

namespace FreeLink.Domain.Ports;

public interface IJwtTokenGenerator
{
    string GenerateToken(int userId, string email, string userType);
}