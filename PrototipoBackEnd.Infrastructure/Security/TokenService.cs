using PrototipoBackEnd.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using PrototipoBackEnd.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace PrototipoBackEnd.Infrastructure.Security
{
	public class TokenService : ITokenService
	{
		#region Construtor
		private readonly IConfiguration _configuration;

		public TokenService(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		#endregion Construtor

		public string GenerateToken(Usuario usuario)
		{
			var claims = new[]
			{
			new Claim(ClaimTypes.Name, usuario.Nome),
			new Claim(ClaimTypes.Email, usuario.Email),
			new Claim(ClaimTypes.Role, usuario.Role.ToString())
		};

			var key = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				claims: claims,
				expires: DateTime.Now.AddMinutes(60),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

	}

}
