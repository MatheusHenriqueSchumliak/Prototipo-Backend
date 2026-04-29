using PrototipoBackEnd.Application.Dtos.Artesanato;
using PrototipoBackEnd.Domain.ValueObjects;

namespace PrototipoBackEnd.Application.Factories
{
	public static class CategoriaFactory
	{
		public static CategoriaDto CriarDto(Categoria categoria)
		{
			return new CategoriaDto
			{
				Itens = categoria.Itens.ToList()
			};
		}

		public static Categoria CriarEntidade(CategoriaDto dto)
		{
			return new Categoria(dto.Itens);
		}

	}
}
