using MongoDB.Driver;
using PrototipoBackEnd.Domain.Entities;

namespace PrototipoBackEnd.Domain.Interfaces.Repositories
{
	public interface IArtesaoRepository : IGenericRepository<Artesao>
	{
		Task<IEnumerable<Artesao>> BuscarComFiltro(FilterDefinition<Artesao> filtro);

	}
}
