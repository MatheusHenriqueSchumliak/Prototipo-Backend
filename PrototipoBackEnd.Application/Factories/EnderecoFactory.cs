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
				dto.Rua,
				dto.Numero,
				dto.Bairro,
				dto.Cidade,
				dto.Estado,
				dto.CEP,
				dto.Complemento,
				dto.SemNumero
			);
		}
	}
}
