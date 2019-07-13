using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Northwind.API.Resources;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.API.Services
{
    public class JwtHandler
    {
        private readonly IConfiguration _configuration;

        public JwtHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //public TokenResource CreateAccessToken(Guid userId, string email, Role role)
        public TokenResource CreateAccessToken(Guid userId, string email)
        {
            var now = DateTime.UtcNow;
            var claims = new Claim[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            //new Claim(ClaimTypes.Role, role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            //new Claim(JwtRegisteredClaimNames.Iat, now.ToTimeStamp().ToString(), ClaimValueTypes.Integer64),
            };

            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"])),
                SecurityAlgorithms.HmacSha256);
            var expiry = now.AddMinutes(15);
            var jwt = CreateSecurityToken(claims, now, expiry, signingCredentials);
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return CreateTokenResource(token, DateTime.UtcNow.AddMinutes(15));
        }

        public TokenResource CreateRefreshToken(Guid userId)
        {
            var now = DateTime.UtcNow;
            var claims = new Claim[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            //new Claim(JwtRegisteredClaimNames.Iat, now.ToTimeStamp().ToString(), ClaimValueTypes.Integer64),
            };

            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"])),
                SecurityAlgorithms.HmacSha256);
            var expiry = now.AddMinutes(double.Parse(_configuration["Tokens:RefreshExpireMinutes"]));
            var jwt = CreateSecurityToken(claims, now, expiry, signingCredentials);
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return CreateTokenResource(token, DateTime.UtcNow.AddMinutes(15));
        }

        private JwtSecurityToken CreateSecurityToken(IEnumerable<Claim> claims, DateTime now, DateTime expiry, SigningCredentials credentials)
            => new JwtSecurityToken(claims: claims, notBefore: now, expires: expiry, signingCredentials: credentials);

        private static TokenResource CreateTokenResource(string token, DateTime expiry)
            => new TokenResource { Token = token, Expiry = expiry };
    }
}
