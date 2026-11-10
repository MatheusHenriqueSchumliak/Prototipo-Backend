using PrototipoBackEnd.Domain.Interfaces.Repositories;
using PrototipoBackEnd.Application.Services;
using PrototipoBackEnd.Domain.ValueObjects;
using PrototipoBackEnd.Domain.Interfaces;
using PrototipoBackEnd.Domain.Entities;
using Xunit;
using Moq;

namespace PrototipoBackEnd.Tests.Unit.Services
{
	public class PessoaServiceTests
	{
		private readonly Mock<IPessoaRepository> _pessoaRepositoryMock;
		private readonly PessoaService _service;

		public PessoaServiceTests()
		{
			_pessoaRepositoryMock = new Mock<IPessoaRepository>();
			_service = new PessoaService(_pessoaRepositoryMock.Object, new Mock<IUsuarioRepository>().Object, new Mock<IArtesaoRepository>().Object, new Mock<IArtesanatoRepository>().Object, new PrototipoBackEnd.Tests.Integration.Repositories.UnitOfWorkFake());
		}

		[Fact]
		public async Task BuscarPorId_DeveRetornarPessoaDto_QuandoEncontrada()
		{
			// Arrange
			var id = Guid.NewGuid().ToString();
			var pessoa = Pessoa.Criar(
				"Fulano de Tal",
				new DateTime(1990, 1, 1),
				new Endereco(
					cep: "12345678",
					estado: "SP",
					cidade: "São Paulo",
					rua: "Rua Exemplo",
					bairro: "Centro",
					numero: "100",
					complemento: "",
					semNumero: false
				),
				new Contato(
					telefone: "11999999999",
					whatsapp: "11999999999",
					email: "fulano@email.com"
				)
			);
			_pessoaRepositoryMock.Setup(r => r.BuscarPorId(id)).ReturnsAsync(pessoa);

			// Act
			var result = await _service.BuscarPorId(id);

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Fulano de Tal", result.NomeCompleto);
			Assert.Equal("12345678", result.Endereco.CEP); // Corrigido para "CEP"
			Assert.Equal("11999999999", result.Contato.Telefone);
			_pessoaRepositoryMock.Verify(r => r.BuscarPorId(id), Times.Once);
		}

		[Fact]
		public async Task BuscarPorId_DeveLancarExcecao_QuandoNaoEncontrada()
		{
			var id = Guid.NewGuid().ToString();
			_pessoaRepositoryMock.Setup(r => r.BuscarPorId(id)).ReturnsAsync((Pessoa?)null!);

			await Assert.ThrowsAsync<PrototipoBackEnd.Application.Exceptions.NotFoundException>(() => _service.BuscarPorId(id));
		}

		[Fact]
		public async Task BuscarPorId_DeveLancarNotFound_QuandoInativa()
		{
			var pessoa = PessoaTestes.Criar();
			pessoa.Inativar();
			_pessoaRepositoryMock.Setup(r => r.BuscarPorId(pessoa.Id)).ReturnsAsync(pessoa);

			await Assert.ThrowsAsync<PrototipoBackEnd.Application.Exceptions.NotFoundException>(() => _service.BuscarPorId(pessoa.Id));
		}

		[Fact]
		public async Task Apagar_FazSoftDeleteEmCascata()
		{
			var pessoa = PessoaTestes.Criar();
			pessoa.VincularUsuario();
			pessoa.TornarArtesao();
			var usuario = Usuario.Criar(pessoa.Id, "a@b.com", "hash", PrototipoBackEnd.Domain.Enumerables.UsuarioEnum.Usuario);
			var artesao = Artesao.Criar(pessoa.Id, "Nome", "Desc", "foto", new Especialidade(new[] { "x" }), null, new RedesSociais(null, null), true, true, false, false);
			var artesanato = Artesanato.Criar(artesao.Id, "T", "D", 1, 0, 10m, false, false, true, new Categoria(new[] { "c" }), new Material(new[] { "m" }), new Midia(new[] { "i" }));

			var usuarios = new Mock<IUsuarioRepository>();
			usuarios.Setup(r => r.BuscarPorPessoaId(pessoa.Id)).ReturnsAsync(usuario);
			usuarios.Setup(r => r.Atualizar(It.IsAny<Usuario>(), It.IsAny<string>())).ReturnsAsync((Usuario u, string _) => u);
			var artesaos = new Mock<IArtesaoRepository>();
			artesaos.Setup(r => r.BuscarPorPessoaId(pessoa.Id)).ReturnsAsync(artesao);
			artesaos.Setup(r => r.Atualizar(It.IsAny<Artesao>(), It.IsAny<string>())).ReturnsAsync((Artesao a, string _) => a);
			var artesanatos = new Mock<IArtesanatoRepository>();
			artesanatos.Setup(r => r.BuscarTodosPorArtesaoId(artesao.Id)).ReturnsAsync(new List<Artesanato> { artesanato });
			artesanatos.Setup(r => r.Atualizar(It.IsAny<Artesanato>(), It.IsAny<string>())).ReturnsAsync((Artesanato a, string _) => a);
			_pessoaRepositoryMock.Setup(r => r.BuscarPorId(pessoa.Id)).ReturnsAsync(pessoa);
			_pessoaRepositoryMock.Setup(r => r.Atualizar(It.IsAny<Pessoa>(), It.IsAny<string>())).ReturnsAsync((Pessoa p, string _) => p);

			var service = new PessoaService(_pessoaRepositoryMock.Object, usuarios.Object, artesaos.Object, artesanatos.Object, new PrototipoBackEnd.Tests.Integration.Repositories.UnitOfWorkFake());

			await service.Apagar(pessoa.Id);

			Assert.False(pessoa.IsAtivo);
			Assert.False(usuario.IsAtivo);
			Assert.False(artesao.IsAtivo);
			Assert.False(artesanato.IsAtivo);
			_pessoaRepositoryMock.Verify(r => r.Deletar(It.IsAny<string>()), Times.Never);
		}
	}
}
