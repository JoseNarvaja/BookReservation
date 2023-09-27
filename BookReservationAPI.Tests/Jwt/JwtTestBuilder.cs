using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BookReservationAPI.Tests.Jwt
{
    public class JwtTestBuilder
    {
        public List<Claim> Claims { get; } = new();
        public int ExpiresInMinutes { get; set; } = 30;

        public JwtTestBuilder WithRole(string roleName)
        {
            Claims.Add(new Claim("role", roleName));
            return this;
        }

        public JwtTestBuilder WithUserName(string username)
        {
            Claims.Add(new Claim(ClaimTypes.Upn, username));
            return this;
        }

        public JwtTestBuilder WithExpiration(int expiresInMinutes)
        {
            ExpiresInMinutes = expiresInMinutes;
            return this;
        }

        public string Build()
        {
            var token = new JwtSecurityToken(
                JwtTokenProvider.Issuer,
                JwtTokenProvider.Issuer,
                Claims,
                expires: DateTime.Now.AddMinutes(ExpiresInMinutes),
                signingCredentials: JwtTokenProvider.SigningCredentials
            );
            return JwtTokenProvider.JwtSecurityTokenHandler.WriteToken(token);
        }
    }
}
