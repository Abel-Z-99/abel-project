using System.Collections.Generic;

namespace Application.Auth
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(
            int userId,
            string username,
            string email,
            IEnumerable<string> roles,
            IEnumerable<string> permissions);
    }
}
