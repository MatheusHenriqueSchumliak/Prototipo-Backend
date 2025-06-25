using PrototipoBackEnd.Application.Dtos;
using Microsoft.AspNetCore.Http;

namespace PrototipoBackEnd.Application.Interfaces
{
	public interface IArtesaoService
	{
		Task<List<ArtesaoDto>> BuscarTodos();
		Task<ArtesaoDto> BuscarPorId(string id);
		Task<ArtesaoDto> Adicionar(ArtesaoDto dto, IFormFile imagem);
		Task<ArtesaoDto> Atualizar(ArtesaoDto dto, string id, IFormFile? imagem = null);
		Task<bool> Apagar(string id);


		Task<List<ArtesaoDto>> BuscarComFiltro(string? nome, string? nichoAtuacao, bool? receberEncomendas, bool? enviaEncomendas);
	}
}
