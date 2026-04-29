using PrototipoBackEnd.Application.Dtos.Auth;

namespace PrototipoBackEnd.Application.Interfaces
{
	public interface IAuthService
	{
		Task<LoginResponseDto> Login(LoginRequestDto request);
	}
}
