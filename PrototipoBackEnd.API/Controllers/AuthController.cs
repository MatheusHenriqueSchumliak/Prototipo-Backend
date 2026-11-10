using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrototipoBackEnd.API.Responses;
using PrototipoBackEnd.Application.Dtos.Auth;
using PrototipoBackEnd.Application.Interfaces;

namespace PrototipoBackEnd.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // ValidationException (400) e UnauthorizedAccessException (401) são convertidas pelo middleware global.
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var result = await _authService.Login(dto);
            return Ok(new ApiResponse<LoginResponseDto>("Login realizado com sucesso", result));
        }
    }
}
