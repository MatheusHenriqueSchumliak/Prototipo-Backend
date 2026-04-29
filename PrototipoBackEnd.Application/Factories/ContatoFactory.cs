using PrototipoBackEnd.Application.Dtos.Pessoa;
using PrototipoBackEnd.Domain.ValueObjects;

namespace PrototipoBackEnd.Application.Factories
{
	public static class ContatoFactory
	{
		public static ContatoDto CriarDto(Contato contato)
		{
			return new ContatoDto
			{
				Telefone = contato.Telefone,
				WhatsApp = contato.WhatsApp,
				Email = contato.Email
			};
		}

		public static Contato CriarEntidade(ContatoDto dto)
		{
			return new Contato(dto.Telefone, dto.WhatsApp, dto.Email);
		}
	}
}
