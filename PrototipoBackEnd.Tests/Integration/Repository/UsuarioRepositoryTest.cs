

using MongoDB.Driver;
using PrototipoBackEnd.Domain.Entities;
using PrototipoBackEnd.Domain.Enumerables;
using PrototipoBackEnd.Domain.Interfaces;
using PrototipoBackEnd.Infrastructure.Repository;
using Xunit;

namespace PrototipoBackEnd.UnitTests.Integration.Repository
{
	public class UsuarioRepositoryTest
	{
		private readonly IUsuarioRepository _repository;

		public UsuarioRepositoryTest()
		{
			// Configuração do MongoDB para testes
			var mongoClient = new MongoClient("mongodb://localhost:27017");
			var database = mongoClient.GetDatabase("PrototipoBackEndTestDb");

			// Instancia o repositório concreto
			_repository = new UsuarioRepository(database, "Usuarios");
		}

		[Fact]
		public async Task Deve_Adicionar_Usuario()
		{
			var usuario = Usuario.Criar(
				nome: "Teste",
				email: "teste@email.com",
				senhaHash: "hash",
				role: UsuarioEnum.Administrador,
				ativo: true
			);

			await _repository.Adicionar(usuario);

			var encontrado = await _repository.BuscarPorId(usuario.Id);
			Assert.NotNull(encontrado);
		}

		[Fact]
		public async Task Deve_Atualizar_Usuario()
		{
			var usuario = Usuario.Criar(
				nome: "Teste",
				email: "teste@email.com",
				senhaHash: "hash",
				role: UsuarioEnum.Administrador,
				ativo: true
			);
			await _repository.Adicionar(usuario);

			usuario.Nome = "Atualizado";
			await _repository.Atualizar(usuario, usuario.Id);

			var atualizado = await _repository.BuscarPorId(usuario.Id);
			Assert.Equal("Atualizado", atualizado.Nome);
		}

		[Fact]
		public async Task Deve_Remover_Usuario()
		{
			var usuario = Usuario.Criar(
				nome: "Teste",
				email: "teste@email.com",
				senhaHash: "hash",
				role: UsuarioEnum.Administrador,
				ativo: true
			);
			await _repository.Adicionar(usuario);

			await _repository.Deletar(usuario.Id);

			var removido = await _repository.BuscarPorId(usuario.Id);
			Assert.Null(removido);
		}

		[Fact]
		public async Task Deve_Buscar_Por_Id()
		{
			var usuario = Usuario.Criar(
				nome: "Teste",
				email: "teste@email.com",
				senhaHash: "hash",
				role: UsuarioEnum.Administrador,
				ativo: true
			);
			await _repository.Adicionar(usuario);

			var encontrado = await _repository.BuscarPorId(usuario.Id);
			Assert.NotNull(encontrado);
		}

		[Fact]
		public async Task Deve_Listar_Todos()
		{
			var usuario1 = Usuario.Criar(
				nome: "Teste1",
				email: "teste1@email.com",
				senhaHash: "hash1",
				role: UsuarioEnum.Administrador,
				ativo: true
			);
			var usuario2 = Usuario.Criar(
				nome: "Teste2",
				email: "teste2@email.com",
				senhaHash: "hash2",
				role: UsuarioEnum.Administrador,
				ativo: true
			);
			await _repository.Adicionar(usuario1);
			await _repository.Adicionar(usuario2);

			var todos = await _repository.BuscarTodos();
			Assert.True(todos.Count() >= 2);
		}
	}
}
