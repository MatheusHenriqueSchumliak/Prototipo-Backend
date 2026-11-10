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

		public async Task<Usuario> ObterUsuarioPorEmail(string email)
		{
			// E-mails são persistidos normalizados em minúsculas (Usuario.Criar/AlterarEmail)
			var emailNormalizado = email.Trim().ToLowerInvariant();

			return await _usuarioCollection
						.Find(u => u.Email == emailNormalizado)
						.FirstOrDefaultAsync();
		}

		public async Task<Usuario?> BuscarPorPessoaId(string pessoaId)
		{
			return await _usuarioCollection.Find(u => u.PessoaId == pessoaId && u.IsAtivo).FirstOrDefaultAsync();
		}

		public async Task GarantirIndicesAsync()
		{
			var indiceEmail = new CreateIndexModel<Usuario>(
				Builders<Usuario>.IndexKeys.Ascending(u => u.Email),
				new CreateIndexOptions { Unique = true, Name = "ux_usuarios_email" });

			await _usuarioCollection.Indexes.CreateOneAsync(indiceEmail);
		}

	}
}
