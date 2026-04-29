using PrototipoBackEnd.Application.Dtos.Artesanato;
using PrototipoBackEnd.Domain.ValueObjects;

namespace PrototipoBackEnd.Application.Factories
{
	public static class MidiaFactory
	{
		public static MidiaDto CriarDto(Midia midia)
		{
			return new MidiaDto
			{
				Imagens = midia.Imagens.ToList()
			};
		}

		public static Midia CriarEntidade(MidiaDto dto)
		{
			return new Midia(dto.Imagens);
		}
	}
}
