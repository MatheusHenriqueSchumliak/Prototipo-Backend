using PrototipoBackEnd.Application.Dtos.Usuario;
using PrototipoBackEnd.Domain.Entities;

namespace PrototipoBackEnd.Application.Factories
{
	public static class UsuarioFactory
	{
		public static UsuarioDto CriarDto(Usuario usuario)
		{
			return new UsuarioDto
			{
				Id = usuario.Id,
				PessoaId = usuario.PessoaId,
				Email = usuario.Email,
				Role = usuario.Role,
				IsAtivo = usuario.IsAtivo
			};
		}

		public static Usuario CriarEntidade(UsuarioDto dto)
		{
			return Usuario.Criar(dto.PessoaId, dto.Email, dto.SenhaHash, dto.Role);
		}
	}
}
