using PrototipoBackEnd.Application.Dtos;

namespace PrototipoBackEnd.Application.Interfaces
{
	public interface IArtesaoService
	{
		Task<List<ArtesaoDto>> BuscarTodos();
		Task<ArtesaoDto> BuscarPorId(string id);
		Task Adicionar(ArtesaoDto dto);
		Task Atualizar(ArtesaoDto dto, string id);
		Task<bool> Apagar(string id);
	}
}
