using Moq;
using PrototipoBackEnd.Application.Services;
using PrototipoBackEnd.Domain.Entities;
using PrototipoBackEnd.Domain.Interfaces.Repositories;
using PrototipoBackEnd.Domain.ValueObjects;
using Xunit;

namespace PrototipoBackEnd.Tests.Unit.Services
{
	public class PessoaServiceTests
	{
		private readonly Mock<IPessoaRepository> _pessoaRepositoryMock;
		private readonly PessoaService _service;

		public PessoaServiceTests()
		{
			_pessoaRepositoryMock = new Mock<IPessoaRepository>();
			_service = new PessoaService(_pessoaRepositoryMock.Object);
		}

		[Fact]
		public async Task BuscarPorId_DeveRetornarPessoaDto_QuandoEncontrada()
		{
			// Arrange
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
			_pessoaRepositoryMock.Setup(r => r.BuscarPorId("1")).ReturnsAsync(pessoa);

			// Act
			var result = await _service.BuscarPorId("1");

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Fulano de Tal", result.NomeCompleto);
			Assert.Equal("12345678", result.Endereco.CEP); // Corrigido para "CEP"
			Assert.Equal("11999999999", result.Contato.Telefone);
			_pessoaRepositoryMock.Verify(r => r.BuscarPorId("1"), Times.Once);
		}

		[Fact]
		public async Task BuscarPorId_DeveLancarExcecao_QuandoNaoEncontrada()
		{
			_pessoaRepositoryMock.Setup(r => r.BuscarPorId("2")).ReturnsAsync((Pessoa?)null!);

			await Assert.ThrowsAsync<Exception>(() => _service.BuscarPorId("2"));
		}
	}
}
