using PrototipoBackEnd.Infrastructure.Mapping;
using PrototipoBackEnd.Infrastructure.Context;
using MongoDB.Driver;
using PrototipoBackEnd.Domain.Interfaces.Repositories;

namespace PrototipoBackEnd.Infrastructure.Repository
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{
		#region Construtor 		
		private readonly IMongoCollection<T> _collection;

		public GenericRepository(MongoDbContext mongoDbContext)
		{
			// Usando o mapeamento para buscar o nome correto da coleção e inicializar a coleção
			var collectionName = MongoCollectionMapper.GetCollectionName<T>();
			_collection = mongoDbContext.GetDatabase().GetCollection<T>(collectionName);
		}
		#endregion

		public async Task<IEnumerable<T>> BuscarTodos()
			=> await _collection.Find(_ => true).ToListAsync();

		public async Task<T> BuscarPorId(string id)
		{
			if (!Guid.TryParse(id, out _))
			{
				throw new ArgumentException("ID inválido.");
			}

			var filter = Builders<T>.Filter.Eq("_id", id);
			return await _collection.Find(filter).FirstOrDefaultAsync();
		}

		public async Task<T> Adicionar(T entity)
		{
			await _collection.InsertOneAsync(entity);
			return entity;
		}

		public async Task<T> Atualizar(T entity, string id)
		{
			if (!Guid.TryParse(id, out _))
				throw new ArgumentException("ID inválido.");


			// Define o filtro pelo ID
			var filter = Builders<T>.Filter.Eq("_id", id);

			// Garante que o ID da entidade seja o mesmo do parâmetro
			// Isso evita que o Mongo entenda como uma tentativa de trocar o _id
			var prop = typeof(T).GetProperty("Id");
			if (prop != null && prop.CanWrite)
			{
				prop.SetValue(entity, id);
			}

			var result = await _collection.ReplaceOneAsync(filter, entity, new ReplaceOptions { IsUpsert = false });

			if (result.MatchedCount == 0)
				throw new Exception($"Entidade com o ID {id} não encontrada.");


			return entity;
		}

		public async Task<bool> Deletar(string id)
		{
			if (!Guid.TryParse(id, out _))
			{
				throw new ArgumentException("ID inválido.");
			}

			var filter = Builders<T>.Filter.Eq("_id", id);
			var result = await _collection.DeleteOneAsync(filter);
			return result.DeletedCount > 0;
		}


	}
}
