using PrototipoBackEnd.Application.Dtos.Pessoa;
using PrototipoBackEnd.Domain.ValueObjects;

namespace PrototipoBackEnd.Application.Factories
{
	public static class EnderecoFactory
	{
		public static EnderecoDto CriarDto(Endereco endereco)
		{
			return new EnderecoDto
			{
				Rua = endereco.Rua,
				Numero = endereco.Numero,
				Bairro = endereco.Bairro,
				Cidade = endereco.Cidade,
				Estado = endereco.Estado,
				CEP = endereco.CEP,
				Complemento = endereco.Complemento,
				SemNumero = endereco.SemNumero
			};
		}

		public static Endereco CriarEntidade(EnderecoDto dto)
		{
			return new Endereco(
				dto.CEP,
				dto.Estado,
				dto.Cidade,
				dto.Rua,
				dto.Bairro,
				dto.Numero,
				dto.Complemento,
				dto.SemNumero
			);
		}
	}
}
