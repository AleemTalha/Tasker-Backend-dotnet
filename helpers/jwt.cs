using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Models;

namespace Helpers
{
    public static class JwtHelper
    {
        public static string GenerateToken(String Email, String FullName, int Id, IConfiguration config)
        {
            try
            {
                var secretKey = config["Jwt:Key"];
                if (string.IsNullOrWhiteSpace(secretKey) || secretKey == null)
                {
                    throw new Exception("Invalid Jwt secret key");
                }
                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(secretKey)
                );

                Console.WriteLine("Jwt Key: " + secretKey + " | " + key.Key.Length);

                var claims = new[]
                {
                    new Claim("Id", Id.ToString()),
                    new Claim("FullName", FullName)
                    ,new Claim("Email", Email),
                };
                var SignOptions = new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256
                );
                var Jwt = new JwtSecurityToken(
                    issuer: config["Jwt:Issuer"],
                    audience: config["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMilliseconds(1000 * 60 * 60 * 24),
                    signingCredentials: SignOptions
                );
                return new JwtSecurityTokenHandler().WriteToken(Jwt);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Jwt generation failed", ex);
            }
        }
    }
}