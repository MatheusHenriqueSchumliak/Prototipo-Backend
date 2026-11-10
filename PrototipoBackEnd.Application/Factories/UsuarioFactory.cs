using PrototipoBackEnd.Application.Dtos.Usuario;
using PrototipoBackEnd.Domain.Entities;

namespace PrototipoBackEnd.Application.Factories
{
	public static class UsuarioFactory
	{
		public static UsuarioResponseDto CriarDto(Usuario usuario)
		{
			return new UsuarioResponseDto
			{
				Id = usuario.Id,
				PessoaId = usuario.PessoaId,
				Email = usuario.Email,
				Role = usuario.Role,
				IsAtivo = usuario.IsAtivo
			};
		}
	}
}
