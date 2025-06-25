using PrototipoBackEnd.Application.Interfaces;
using PrototipoBackEnd.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace PrototipoBackEnd.API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		#region Construtor
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}
		#endregion

		[HttpPost]
		public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
		{
			try
			{
				var result = await _authService.Login(dto);
				return Ok(result);
			}
			catch (UnauthorizedAccessException ex)
			{
				return Unauthorized(new { message = ex.Message });
			}
		}

	}
}
