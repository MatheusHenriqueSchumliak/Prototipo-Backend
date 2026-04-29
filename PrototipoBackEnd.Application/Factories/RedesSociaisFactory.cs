using PrototipoBackEnd.Application.Dtos.Artesao;
using PrototipoBackEnd.Domain.ValueObjects;

namespace PrototipoBackEnd.Application.Factories
{
	public static class RedesSociaisFactory
	{
		public static RedesSociaisDto CriarDto(RedesSociais redes)
		{
			return new RedesSociaisDto
			{
				Instagram = redes.Instagram,
				Facebook = redes.Facebook
			};
		}

		public static RedesSociais CriarEntidade(RedesSociaisDto dto)
		{
			return new RedesSociais(dto.Instagram, dto.Facebook);
		}
	}
}
