using PrototipoBackEnd.Domain.Entities;

namespace PrototipoBackEnd.Domain.Interfaces.Services
{
	public interface ITokenService
	{
		string GenerateToken(Usuario usuario);
	}
}
