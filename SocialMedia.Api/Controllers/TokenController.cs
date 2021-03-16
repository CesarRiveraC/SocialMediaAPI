using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SocialMedia.Core.Entities;

namespace SocialMedia.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TokenController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Authentication(UserLogin login)
        {
            if (isValidUser(login))
            {
                var token = GenerateToken();

                return Ok(new
                {
                    token
                });
            }

            return NotFound();
        }

        private string GenerateToken()
        {
            //Headers
            var symmetricSecurityKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var header = new JwtHeader(signingCredentials);


            //Claims
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "Cesar Rivera"),
                new Claim(ClaimTypes.Email, "cesarrivcam@gmail.com"),
                new Claim(ClaimTypes.Role, "Administrador"),
            };

            var payLoad = new JwtPayload
            (
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claims,
                DateTime.Now,
                DateTime.UtcNow.AddMinutes(5)
            );

            var token = new JwtSecurityToken(header, payLoad);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool isValidUser(UserLogin login)
        {
            return true;
        }
    }
}