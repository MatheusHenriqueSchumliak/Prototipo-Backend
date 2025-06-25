using PrototipoBackEnd.Domain.Entities;

namespace PrototipoBackEnd.Domain.Interfaces.Repositories
{
	public interface IArtesanatoRepository : IGenericRepository<Artesanato>
	{
		Task<Artesanato> BuscarPorArtesaoId(string artesaoId);
		Task<List<Artesanato>> BuscarTodosPorArtesaoId(string artesaoId);
	}
}
