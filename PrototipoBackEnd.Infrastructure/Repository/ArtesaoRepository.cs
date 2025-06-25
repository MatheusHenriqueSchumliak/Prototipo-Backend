using PrototipoBackEnd.Domain.Interfaces.Repositories;
using PrototipoBackEnd.Infrastructure.Context;
using PrototipoBackEnd.Domain.Entities;
using MongoDB.Driver;
using MongoDB.Bson;

namespace PrototipoBackEnd.Infrastructure.Repository
{
	public class ArtesaoRepository : GenericRepository<Artesao>, IArtesaoRepository
	{
		#region Construtor
		private readonly IMongoCollection<Artesao> _artesaoCollection;
		public ArtesaoRepository(MongoDbContext mongoDbContext) : base(mongoDbContext)
		{
			_artesaoCollection = mongoDbContext.GetDatabase().GetCollection<Artesao>("Artesaos");
		}
		#endregion

		public async Task<IEnumerable<Artesao>> BuscarComFiltro(FilterDefinition<Artesao> filtro)
		{
			return await _artesaoCollection.Find(filtro).ToListAsync();
		}
	}
}
