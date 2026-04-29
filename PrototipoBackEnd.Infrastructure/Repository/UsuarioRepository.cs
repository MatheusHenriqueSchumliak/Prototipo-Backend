using PrototipoBackEnd.Domain.Interfaces.Repositories;
using PrototipoBackEnd.Domain.Interfaces;
using PrototipoBackEnd.Domain.Entities;
using MongoDB.Driver;

namespace PrototipoBackEnd.Infrastructure.Repository
{
	public class UsuarioRepository : GenericRepository<Usuario>, IUsuarioRepository, IGenericRepository<Usuario>

	{
		#region Construtor
		private readonly IMongoCollection<Usuario> _usuarioCollection;
		public UsuarioRepository(IMongoDatabase database, string collectionName) : base(database, collectionName)
		{
			_usuarioCollection = database.GetCollection<Usuario>(collectionName);
		}
		#endregion

		public async Task<Usuario> ObterUsuarioPorEmail(string email)
		{
			return await _usuarioCollection
						.Find(u => u.Email.ToLower() == email.ToLower())
						.FirstOrDefaultAsync();
		}

	}
}
