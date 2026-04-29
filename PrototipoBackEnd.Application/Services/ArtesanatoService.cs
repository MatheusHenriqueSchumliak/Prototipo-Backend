using PrototipoBackEnd.Domain.Interfaces.Repositories;
using PrototipoBackEnd.Application.Dtos.Artesanato;
using PrototipoBackEnd.Domain.Interfaces.Services;
using PrototipoBackEnd.Application.Interfaces;
using PrototipoBackEnd.Application.Factories;
using PrototipoBackEnd.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace PrototipoBackEnd.Application.Services
{
	public class ArtesanatoService : IArtesanatoService
	{
		#region Construtor
		private readonly IArtesanatoRepository _artesanatoRepository;
		private readonly IAmazonS3Service _amazonS3Service;

		public ArtesanatoService(IArtesanatoRepository artesanatoRepository, IAmazonS3Service amazonS3Service)
		{
			_artesanatoRepository = artesanatoRepository;
			_amazonS3Service = amazonS3Service;

		}
		#endregion

		public async Task<List<ArtesanatoDto>> BuscarTodos()
		{
			var artesanatos = await _artesanatoRepository.BuscarTodos();
			return artesanatos.Select(ArtesanatoFactory.CriarDto).ToList();
		}
		public async Task<ArtesanatoDto> BuscarPorId(string id)
		{
			try
			{
				var artesanato = await _artesanatoRepository.BuscarPorId(id);
				if (artesanato == null)
					throw new Exception($"Artesato não foi encontrado!");

				return ArtesanatoFactory.CriarDto(artesanato);
			}
			catch (Exception)
			{
				throw new Exception($"Artesato não foi encontrado!");
			}
		}
		public async Task<ArtesanatoDto> BuscarPorArtesaoId(string artesaoId)
		{
			try
			{
				var artesanato = await _artesanatoRepository.BuscarPorArtesaoId(artesaoId);
				if (artesanato == null)
					throw new Exception($"Artesato não foi encontrado!");

				return ArtesanatoFactory.CriarDto(artesanato);
			}
			catch (Exception)
			{
				throw new Exception($"Artesato não foi encontrado!");
			}
		}
		public async Task<List<ArtesanatoDto>> BuscarTodosPorArtesaoId(string artesaoId)
		{
			try
			{
				var artesanatos = await _artesanatoRepository.BuscarTodosPorArtesaoId(artesaoId);
				if (artesanatos == null || artesanatos.Count == 0)
					return new List<ArtesanatoDto>(); // Retorna lista vazia em vez de exception

				return artesanatos.Select(ArtesanatoFactory.CriarDto).ToList();
			}
			catch (Exception)
			{
				throw new Exception($"Erro ao buscar artesanatos!");
			}
		}
		public async Task<ArtesanatoDto> Adicionar(ArtesanatoDto dto, List<IFormFile> imagem)
		{
			if (imagem != null && imagem.Count > 0)
			{
				var imagemUrls = new List<string>();

				foreach (var img in imagem)
				{
					using var stream = img.OpenReadStream();
					var imagemUrl = await _amazonS3Service.Upload(stream, img.FileName, img.ContentType);
					imagemUrls.Add(imagemUrl);
				}

				dto.Midia = new MidiaDto { Imagens = imagemUrls }; // Adiciona todas as URLs como lista  
			}

			var entidade = ArtesanatoFactory.CriarEntidade(dto); // Mapeia o DTO para a entidade de domínio  

			await _artesanatoRepository.Adicionar(entidade); // Salva no banco de dados (ou MongoDB)  

			return ArtesanatoFactory.CriarDto(entidade); // Retorna o DTO já com ID gerado, etc.  
		}
		public async Task Atualizar(ArtesanatoDto dto, string id)
		{
			if (id == null)
				throw new Exception($"Artesão para o ID: {id} não foi encontrado!");

			var artesanatoExistente = await _artesanatoRepository.BuscarPorId(id);

			if (artesanatoExistente == null)
				throw new Exception($"Artesão com o ID: {id} não foi encontrado!");
		}
		public async Task<bool> Apagar(string id)
		{
			if (id == null)
			{
				throw new Exception($"Artesanato para o ID: {id} não foi encontrado!");
			}

			await _artesanatoRepository.Deletar(id);
			return true;
		}
	}
}
