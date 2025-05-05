namespace PrototipoBackEnd.Application.Interfaces
{
	public interface ISenhaService
	{
		string CriarHash(string senha);
		bool VerificarHash(string senha, string hash);
	}
}
