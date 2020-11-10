using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System;
using System.Text;

using Kraken.Api.Main.Models;

namespace Kraken.Api.Main.Services
{
    /// <summary>
    /// Used for generating authentication token.
    /// </summary>
    public class TokenService
    {
        /// <summary>
        /// Program configuration in the form of a key-value set.
        /// </summary>
        public IConfiguration _configuration;

        /// <summary>
        /// Instantiates the Token service.
        /// </summary>
        /// <param name="configuration">Program configuration.</param>
        public TokenService(IConfiguration configuration) => _configuration = configuration;

        /// <summary>
        /// Generates a new token. Note that this method is heavily inspired by the example provided here:
        /// https://www.c-sharpcorner.com/article/jwt-json-web-token-authentication-in-asp-net-core/
        /// 
        /// While generating the token, information like the user's id, email and username are taken into
        /// consideration. When this token is validated by the middleware during requests, this information
        /// is then verified.
        /// </summary>
        /// <param name="user">User information.</param>
        /// <returns>An authentication token.</returns>
        public string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:JwtKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // The user's id, email and username are verified during requests.
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Username)
            };

            double numberOfDays;
            if (!Double.TryParse(_configuration["Authentication:JwtExpireDays"], out numberOfDays))
            {
                numberOfDays = 30;
            }

            var token = new JwtSecurityToken
                (_configuration["Authentication:JwtIssuer"],
                    _configuration["Authentication:JwtIssuer"],
                    claims,
                    expires: DateTime.Now.AddDays(numberOfDays),
                    signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}