using PrototipoBackEnd.Application.Dtos.Pessoa;

namespace PrototipoBackEnd.Application.Interfaces
{
	public interface IPessoaService
	{
		Task<List<PessoaDto>> BuscarTodos();
		Task<PessoaDto> BuscarPorId(string id);
		Task<PessoaDto> Adicionar(PessoaDto dto);
		Task<PessoaDto> Atualizar(PessoaDto dto, string id);
		Task<bool> Apagar(string id);
	}
}
