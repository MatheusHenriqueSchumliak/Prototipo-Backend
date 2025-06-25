using PrototipoBackEnd.Domain.Interfaces.Repositories;
using PrototipoBackEnd.Domain.Interfaces.Services;
using PrototipoBackEnd.Application.Interfaces;
using PrototipoBackEnd.Application.Dtos;
using PrototipoBackEnd.Domain.Entities;
using Microsoft.AspNetCore.Http;
using AutoMapper;

namespace PrototipoBackEnd.Application.Services
{
	public class ArtesanatoService : IArtesanatoService
	{
		#region Construtor
		private readonly IArtesanatoRepository _artesanatoRepository;
		private readonly IAmazonS3Service _amazonS3Service;
		private readonly IMapper _mapper;
		public ArtesanatoService(IArtesanatoRepository artesanatoRepository, IAmazonS3Service amazonS3Service, IMapper mapper)
		{
			_artesanatoRepository = artesanatoRepository;
			_amazonS3Service = amazonS3Service;
			_mapper = mapper;
		}
		#endregion

		public async Task<List<ArtesanatoDto>> BuscarTodos()
		{
			var artesanatos = await _artesanatoRepository.BuscarTodos();
			return _mapper.Map<List<ArtesanatoDto>>(artesanatos);
		}
		public async Task<ArtesanatoDto> BuscarPorId(string id)
		{
			try
			{
				var artesanato = await _artesanatoRepository.BuscarPorId(id);
				if (artesanato == null)
					throw new Exception($"Artesato não foi encontrado!");

				var dto = _mapper.Map<ArtesanatoDto>(artesanato);

				return dto;
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

				var dto = _mapper.Map<ArtesanatoDto>(artesanato);

				return dto;
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
				if (artesanatos == null || !artesanatos.Any())
					return new List<ArtesanatoDto>(); // Retorna lista vazia em vez de exception

				var dtos = _mapper.Map<List<ArtesanatoDto>>(artesanatos);
				return dtos;
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

				dto.ImagemUrl = imagemUrls; // Adiciona todas as URLs como lista  
			}

			var entidade = _mapper.Map<Artesanato>(dto); // Mapeia o DTO para a entidade de domínio  

			await _artesanatoRepository.Adicionar(entidade); // Salva no banco de dados (ou MongoDB)  

			return _mapper.Map<ArtesanatoDto>(entidade); // Retorna o DTO já com ID gerado, etc.  
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
