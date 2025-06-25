using PrototipoBackEnd.Application.Dtos;
using Microsoft.AspNetCore.Http;

namespace PrototipoBackEnd.Application.Interfaces
{
	public interface IArtesanatoService
	{
		Task<List<ArtesanatoDto>> BuscarTodos();
		Task<ArtesanatoDto> BuscarPorId(string id);
		Task<ArtesanatoDto> BuscarPorArtesaoId(string artesaoId);
		Task<List<ArtesanatoDto>> BuscarTodosPorArtesaoId(string artesaoId);
		Task<ArtesanatoDto> Adicionar(ArtesanatoDto dto, List<IFormFile> imagem);
		Task Atualizar(ArtesanatoDto dto, string id);
		Task<bool> Apagar(string id);
	}
}
