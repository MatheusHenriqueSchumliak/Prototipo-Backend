using PrototipoBackEnd.Domain.Interfaces.Repositories;
using PrototipoBackEnd.Domain.Entities;

namespace PrototipoBackEnd.Domain.Interfaces
{
	public interface IUsuarioRepository : IGenericRepository<Usuario>
	{
		Task<Usuario> ObterUsuarioPorEmail(string email);
	}
}
