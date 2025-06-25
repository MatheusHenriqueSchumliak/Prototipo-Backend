using PrototipoBackEnd.Application.Dtos;

namespace PrototipoBackEnd.Application.Interfaces
{
	public interface IAuthService
	{
		Task<LoginResponseDto> Login(LoginRequestDto request);
	}
}
