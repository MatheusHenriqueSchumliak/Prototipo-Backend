using PrototipoBackEnd.Application.Dtos.Usuario;

namespace PrototipoBackEnd.Application.Interfaces
{
	public interface IUsuarioService
	{
		Task<List<UsuarioResponseDto>> BuscarTodos();
		Task<UsuarioResponseDto> BuscarPorId(string id);
		Task<UsuarioResponseDto> Adicionar(UsuarioRequestDto dto);
		Task Atualizar(UsuarioAtualizarRequestDto dto, string id);
		Task<bool> Apagar(string id);

	}
}
