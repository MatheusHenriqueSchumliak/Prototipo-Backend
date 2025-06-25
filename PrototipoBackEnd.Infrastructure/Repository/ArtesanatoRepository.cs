using PrototipoBackEnd.Domain.Interfaces.Repositories;
using PrototipoBackEnd.Infrastructure.Context;
using PrototipoBackEnd.Domain.Entities;
using MongoDB.Driver;

namespace PrototipoBackEnd.Infrastructure.Repository
{
	public class ArtesanatoRepository : GenericRepository<Artesanato>, IArtesanatoRepository
	{
		#region Construtor
		private readonly IMongoCollection<Artesanato> _artesaoCollection;
		public ArtesanatoRepository(MongoDbContext mongoDbContext) : base(mongoDbContext)
		{
			_artesaoCollection = mongoDbContext.GetDatabase().GetCollection<Artesanato>("Artesanatos");
		}
		#endregion

		public async Task<Artesanato> BuscarPorArtesaoId(string artesaoId)
		{
			if (!Guid.TryParse(artesaoId, out _))
			{
				throw new ArgumentException("ID do artesanato inválido.");
			}

			var filter = Builders<Artesanato>.Filter.Eq(x =>x.ArtesaoId, artesaoId);
			return await _artesaoCollection.Find(filter).FirstOrDefaultAsync();
		}

		public async Task<List<Artesanato>> BuscarTodosPorArtesaoId(string artesaoId)
		{
			if (!Guid.TryParse(artesaoId, out _))
			{
				throw new ArgumentException("ID do artesão inválido.");
			}

			var filter = Builders<Artesanato>.Filter.Eq(x => x.ArtesaoId, artesaoId);
			return await _artesaoCollection.Find(filter).ToListAsync();
		}
	}
}
