using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Auth
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtSettings _settings;

        public JwtTokenGenerator(IOptions<JwtSettings> options)
        {
            _settings = options.Value;
        }

        public string GenerateToken(
            int userId,
            string username,
            string email,
            IEnumerable<string> roles,
            IEnumerable<string> permissions)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, username),
                new Claim(JwtRegisteredClaimNames.Email, email)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            foreach (var permission in permissions)
            {
                claims.Add(new Claim("permission", permission));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(_settings.ExpirationMinutes);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expires,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
