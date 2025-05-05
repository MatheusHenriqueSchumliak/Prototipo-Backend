using PrototipoBackEnd.Application.Dtos;

namespace PrototipoBackEnd.Application.Interfaces
{
	public interface IUsuarioService
	{
		Task<List<UsuarioDto>> BuscarTodos();
		Task<UsuarioDto> BuscarPorId(string id);
		Task Adicionar(UsuarioDto dto);
		Task Atualizar(UsuarioDto dto, string id);
		Task<bool> Apagar(string id);

	}
}
