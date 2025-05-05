using PrototipoBackEnd.Application.Interfaces;

namespace PrototipoBackEnd.Application.Services
{
	public class SenhaService : ISenhaService
	{
		public string CriarHash(string senha)
		{
			return BCrypt.Net.BCrypt.HashPassword(senha);
		}

		public bool VerificarHash(string senha, string hash)
		{
			return BCrypt.Net.BCrypt.Verify(senha, hash);
		}
	}
}
