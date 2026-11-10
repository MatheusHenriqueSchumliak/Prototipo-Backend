using PrototipoBackEnd.Domain.Interfaces.Repositories;
using PrototipoBackEnd.Application.Dtos.Artesanato;
using PrototipoBackEnd.Application.Common.Helpers;
using PrototipoBackEnd.Application.Exceptions;
using PrototipoBackEnd.Domain.Interfaces.Services;
using PrototipoBackEnd.Application.Interfaces;
using PrototipoBackEnd.Application.Factories;
using PrototipoBackEnd.Domain.Entities;
using PrototipoBackEnd.Domain.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace PrototipoBackEnd.Application.Services
{
	public class ArtesanatoService : IArtesanatoService
	{
		#region Construtor
		private readonly IArtesanatoRepository _artesanatoRepository;
		private readonly IArtesaoRepository _artesaoRepository;
		private readonly IAmazonS3Service _amazonS3Service;

		public ArtesanatoService(IArtesanatoRepository artesanatoRepository, IArtesaoRepository artesaoRepository, IAmazonS3Service amazonS3Service)
		{
			_artesanatoRepository = artesanatoRepository;
			_artesaoRepository = artesaoRepository;
			_amazonS3Service = amazonS3Service;

		}
		#endregion

		public async Task<List<ArtesanatoDto>> BuscarTodos()
		{
			var artesanatos = await _artesanatoRepository.BuscarTodos();
			return artesanatos.Where(a => a.IsAtivo).Select(ArtesanatoFactory.CriarDto).ToList();
		}
		public async Task<ArtesanatoDto> BuscarPorId(string id)
		{
			IdValidator.Validar(id);
			var artesanato = await _artesanatoRepository.BuscarPorId(id);
			if (artesanato == null || !artesanato.IsAtivo)
				throw new NotFoundException("Artesanato não foi encontrado.");

			return ArtesanatoFactory.CriarDto(artesanato);
		}
		public async Task<ArtesanatoDto> BuscarPorArtesaoId(string artesaoId)
		{
			IdValidator.Validar(artesaoId);
			var artesanato = await _artesanatoRepository.BuscarPorArtesaoId(artesaoId);
			if (artesanato == null)
				throw new NotFoundException("Artesanato não foi encontrado.");

			return ArtesanatoFactory.CriarDto(artesanato);
		}
		public async Task<List<ArtesanatoDto>> BuscarTodosPorArtesaoId(string artesaoId)
		{
			IdValidator.Validar(artesaoId);
			var artesanatos = await _artesanatoRepository.BuscarTodosPorArtesaoId(artesaoId);
			if (artesanatos == null || artesanatos.Count == 0)
				return new List<ArtesanatoDto>();

			return artesanatos.Select(ArtesanatoFactory.CriarDto).ToList();
		}
		public async Task<ArtesanatoDto> Adicionar(ArtesanatoDto dto, List<IFormFile> imagem)
		{
			IdValidator.Validar(dto.ArtesaoId, "ArtesaoId inválido.");
			var artesao = await _artesaoRepository.BuscarPorId(dto.ArtesaoId);
			if (artesao == null || !artesao.IsAtivo)
				throw new NotFoundException("Artesão não foi encontrado.");

			if (imagem != null && imagem.Count > 0)
			{
				dto.Midia = new MidiaDto { Imagens = await EnviarImagens(imagem) };
			}

			var entidade = ArtesanatoFactory.CriarEntidade(dto);

			await _artesanatoRepository.Adicionar(entidade);

			return ArtesanatoFactory.CriarDto(entidade);
		}
		public async Task Atualizar(ArtesanatoDto dto, string id, List<IFormFile>? imagem = null)
		{
			IdValidator.Validar(id);

			var artesanatoExistente = await _artesanatoRepository.BuscarPorId(id);

			if (artesanatoExistente == null || !artesanatoExistente.IsAtivo)
				throw new NotFoundException("Artesanato não foi encontrado.");

			// Sem novas imagens, mantém as atuais; com novas, substitui a lista (as antigas não são apagadas do S3).
			Midia? novaMidia = null;
			if (imagem != null && imagem.Count > 0)
				novaMidia = new Midia(await EnviarImagens(imagem));

			artesanatoExistente.Atualizar(
				dto.Titulo,
				dto.Descricao,
				dto.TempoCriacao,
				dto.Quantidade,
				dto.Preco,
				dto.TemEstoque,
				dto.SomenteEncomenda,
				dto.AceitaEncomenda,
				dto.Categoria?.Itens?.Count > 0 ? CategoriaFactory.CriarEntidade(dto.Categoria) : null,
				dto.Material?.Materiais?.Count > 0 ? MaterialFactory.CriarEntidade(dto.Material) : null,
				novaMidia);

			await _artesanatoRepository.Atualizar(artesanatoExistente, id);
		}

		private async Task<List<string>> EnviarImagens(List<IFormFile> imagens)
		{
			// Valida todas antes de enviar qualquer uma, para não deixar arquivos órfãos no S3
			ImagemUploadValidator.Validar(imagens);

			var urls = new List<string>();
			foreach (var img in imagens)
			{
				using var stream = img.OpenReadStream();
				urls.Add(await _amazonS3Service.Upload(stream, S3FileHelper.GerarCaminhoArquivo("artesanatos", img.FileName), img.ContentType));
			}
			return urls;
		}
		public async Task<bool> Apagar(string id)
		{
			IdValidator.Validar(id);

			var artesanato = await _artesanatoRepository.BuscarPorId(id);
			if (artesanato == null || !artesanato.IsAtivo)
				throw new NotFoundException("Artesanato não foi encontrado.");

			artesanato.Inativar();
			await _artesanatoRepository.Atualizar(artesanato, id);
			return true;
		}
	}
}
