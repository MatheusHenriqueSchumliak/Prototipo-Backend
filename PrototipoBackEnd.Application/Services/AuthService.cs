using PrototipoBackEnd.Domain.Interfaces.Services;
using PrototipoBackEnd.Application.Interfaces;
using PrototipoBackEnd.Domain.Interfaces;
using PrototipoBackEnd.Application.Dtos;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;

namespace PrototipoBackEnd.Application.Services
{
	public class AuthService : IAuthService
	{
		#region Construtor
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IUsuarioRepository _usuarioRepository;
		private readonly ITokenService _tokenService;
		// Simulando armazenamento em memória dos refresh tokens
		private static readonly Dictionary<string, string> RefreshTokens = new();
		public AuthService(IHttpContextAccessor httpContextAccessor, IUsuarioRepository usuarioRepository, ITokenService tokenService)
		{
			_httpContextAccessor = httpContextAccessor;
			_usuarioRepository = usuarioRepository;
			_tokenService = tokenService;
		}
		#endregion

		private string GenerateRefreshToken()
		{
			var randomBytes = new byte[32];
			using (var rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(randomBytes);
				return Convert.ToBase64String(randomBytes);
			}
		}

		public async Task<LoginResponseDto> Login(LoginRequestDto request)
		{
			var usuario = await _usuarioRepository.ObterUsuarioPorEmail(request.Email);

			if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
				throw new UnauthorizedAccessException("Credenciais inválidas.");

			var accessToken = _tokenService.GenerateToken(usuario);
			var refreshToken = GenerateRefreshToken();

			// Salva refresh token em memória
			RefreshTokens[usuario.Email] = refreshToken;

			// Adiciona o cookie HttpOnly com o refresh token
			_httpContextAccessor.HttpContext.Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.Strict,
				Expires = DateTime.UtcNow.AddDays(7)
			});

			return new LoginResponseDto
			{
				Token = accessToken,
				Nome = usuario.Nome,
				Email = usuario.Email,
				Role = usuario.Role.ToString()
			};
			//var usuario = await _usuarioRepository.ObterUsuarioPorEmail(request.Email);

			//if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
			//	throw new UnauthorizedAccessException("Credenciais inválidas.");

			//var token = _tokenService.GenerateToken(usuario);

			//return new LoginResponseDto
			//{
			//	Token = token,
			//	Nome = usuario.Nome,
			//	Email = usuario.Email,
			//	Role = usuario.Role.ToString()
			//};
		}

	}
}
