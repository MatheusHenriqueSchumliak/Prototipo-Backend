using PrototipoBackEnd.Application.Dtos.Artesao;
using PrototipoBackEnd.Domain.ValueObjects;

namespace PrototipoBackEnd.Application.Factories
{
	public static class EspecialidadeFactory
	{
		public static EspecialidadeDto CriarDto(Especialidade especialidade)
		{
			return new EspecialidadeDto
			{
				Itens = especialidade.Itens.ToList()
			};
		}

		public static Especialidade CriarEntidade(EspecialidadeDto dto)
		{
			return new Especialidade(dto.Itens);
		}
	}
}
