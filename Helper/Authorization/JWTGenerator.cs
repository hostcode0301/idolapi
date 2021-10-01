using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace idolapi.Helper.Authorization
{
    public class JWTGenerator : IJWTGenerator
    {
        private readonly string key = "Do you think I put my key here ????";
        public JWTGenerator(string key)
        {
            this.key = key;
        }

        /// <summary>
        /// Generate JWT access token
        /// </summary>
        /// <param name="claims">User claims</param>
        /// <returns>JWT token / null (cannot generate)</returns>
        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            // Declare token and properties
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenKey = Encoding.ASCII.GetBytes(key);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature
                )
            };

            // Generate token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}