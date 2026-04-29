using PrototipoBackEnd.Application.Dtos.Artesanato;
using PrototipoBackEnd.Domain.ValueObjects;

namespace PrototipoBackEnd.Application.Factories
{
	public static class MaterialFactory
	{
		public static MaterialDto CriarDto(Material material)
		{
			return new MaterialDto
			{
				Materiais = material.Materiais.ToList()
			};
		}

		public static Material CriarEntidade(MaterialDto dto)
		{
			return new Material(dto.Materiais);
		}
	}
}
