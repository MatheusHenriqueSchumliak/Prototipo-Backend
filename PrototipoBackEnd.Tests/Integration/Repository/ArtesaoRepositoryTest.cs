using PrototipoBackEnd.Domain.Interfaces.Repositories;
using PrototipoBackEnd.Infrastructure.Repository;
using PrototipoBackEnd.Domain.Entities;
using MongoDB.Driver;
using Xunit;

namespace PrototipoBackEnd.UnitTests.Integration.Repository
{
	public class ArtesaoRepositoryTest
	{
		private readonly IArtesaoRepository _repository;

		public ArtesaoRepositoryTest()
		{
			var mongoClient = new MongoClient("mongodb://localhost:27017");
			var database = mongoClient.GetDatabase("PrototipoBackEndTestDb");
			_repository = new ArtesaoRepository(database, "Artesaos");
		}

		[Fact]
		public async Task Deve_Adicionar_Artesao()
		{
			var artesao = Artesao.Criar(
				nome: "Artesão Teste",
				email: "artesao@email.com",
				telefone: "11999999999"
			);

			await _repository.Adicionar(artesao);

			var encontrado = await _repository.BuscarPorId(artesao.Id);
			Assert.NotNull(encontrado);
		}

		[Fact]
		public async Task Deve_Atualizar_Artesao()
		{
			var artesao = Artesao.Criar(
				nome: "Artesão Teste",
				email: "artesao@email.com",
				telefone: "11999999999"
			);
			await _repository.Adicionar(artesao);

			artesao.Nome = "Atualizado";
			await _repository.Atualizar(artesao, artesao.Id);

			var atualizado = await _repository.BuscarPorId(artesao.Id);
			Assert.Equal("Atualizado", atualizado.Nome);
		}

		[Fact]
		public async Task Deve_Remover_Artesao()
		{
			var artesao = Artesao.Criar(
				nomeArtesao: "Artesão Teste",
				email: "artesao@email.com",
				telefone: "11999999999"
			);
			await _repository.Adicionar(artesao);

			await _repository.Deletar(artesao.Id);

			var removido = await _repository.BuscarPorId(artesao.Id);
			Assert.Null(removido);
		}

		[Fact]
		public async Task Deve_Buscar_Por_Id()
		{
			var artesao = Artesao.Criar(
				nome: "Artesão Teste",
				email: "artesao@email.com",
				telefone: "11999999999"
			);
			await _repository.Adicionar(artesao);

			var encontrado = await _repository.BuscarPorId(artesao.Id);
			Assert.NotNull(encontrado);
		}

		[Fact]
		public async Task Deve_Listar_Todos()
		{
			var artesao1 = Artesao.Criar(
				nome: "Artesão 1",
				email: "artesao1@email.com",
				telefone: "11999999998"
			);
			var artesao2 = Artesao.Criar(
				nome: "Artesão 2",
				email: "artesao2@email.com",
				telefone: "11999999997"
			);
			await _repository.Adicionar(artesao1);
			await _repository.Adicionar(artesao2);

			var todos = await _repository.BuscarTodos();
			Assert.True(todos.Count() >= 2);
		}
	}
}
