using PrototipoBackEnd.Domain.Interfaces.Repositories;
using PrototipoBackEnd.Tests.Integration.Repositories;
using PrototipoBackEnd.Application.Dtos.Pessoa;
using PrototipoBackEnd.Application.Services;
using Xunit;

namespace PrototipoBackEnd.Tests.Integration.Services
{
	public class PessoaServiceIntegrationTests
	{
		// Supondo que você tenha um repositório fake ou em memória
		private readonly IPessoaRepository _pessoaRepository;
		private readonly PessoaService _service;

		public PessoaServiceIntegrationTests()
		{
			_pessoaRepository = new PessoaRepositoryInMemory(); // Implemente conforme sua necessidade
			_service = new PessoaService(_pessoaRepository);
		}

		[Fact]
		public async Task Adicionar_Integracao_DevePersistirPessoa()
		{
			var dto = new PessoaDto
			{
				NomeCompleto = "Teste",
				DataNascimento = DateTime.Now.AddYears(-20),
				Endereco = new EnderecoDto
				{
					CEP = "23145-678",
					Estado = "SP",
					Cidade = "São Paulo",
					Rua = "Rua Exemplo",
					Bairro = "Centro",
					Numero = "100",
					Complemento = "",
					SemNumero = false
				},
				Contato = new ContatoDto
				{
					Telefone = "11999999999",
					WhatsApp = "11999999999",
					Email = "teste@email.com"
				}
			};

			var result = await _service.Adicionar(dto);

			Assert.NotNull(result);
			Assert.Equal("Teste", result.NomeCompleto);
		}
	}
}
