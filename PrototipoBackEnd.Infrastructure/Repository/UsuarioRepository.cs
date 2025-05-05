using PrototipoBackEnd.Infrastructure.Context;
using PrototipoBackEnd.Domain.Interfaces;
using PrototipoBackEnd.Domain.Entities;
using MongoDB.Driver;

namespace PrototipoBackEnd.Infrastructure.Repository
{
	public class UsuarioRepository : GenericRepository<Usuario>, IUsuarioRepository
	{
		#region Construtor
		private readonly IMongoCollection<Usuario> _usuarioCollection;
		public UsuarioRepository(MongoDbContext mongoDbContext) : base(mongoDbContext)
		{
			_usuarioCollection = mongoDbContext.GetDatabase().GetCollection<Usuario>("Usuarios");
		}
		#endregion


	}
}
