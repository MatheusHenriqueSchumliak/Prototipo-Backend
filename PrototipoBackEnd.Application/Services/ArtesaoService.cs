using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using PrototipoBackEnd.Application.Common.Helpers;
using PrototipoBackEnd.Application.Dtos.Artesao;
using PrototipoBackEnd.Application.Exceptions;
using PrototipoBackEnd.Application.Factories;
using PrototipoBackEnd.Application.Interfaces;
using PrototipoBackEnd.Domain.Entities;
using PrototipoBackEnd.Domain.Interfaces.Repositories;
using PrototipoBackEnd.Domain.Interfaces;
using PrototipoBackEnd.Domain.Interfaces.Services;
using PrototipoBackEnd.Domain.ValueObjects;

namespace PrototipoBackEnd.Application.Services
{
	public class ArtesaoService : IArtesaoService
	{
		#region Construtor
		private readonly IArtesaoRepository _artesaoRepository;
		private readonly IPessoaRepository _pessoaRepository;
		private readonly IArtesanatoRepository _artesanatoRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IAmazonS3Service _amazonS3Service;
		public ArtesaoService(IArtesaoRepository artesaoRepository, IPessoaRepository pessoaRepository, IArtesanatoRepository artesanatoRepository, IUnitOfWork unitOfWork, IAmazonS3Service amazonS3Service)
		{
			_artesaoRepository = artesaoRepository;
			_pessoaRepository = pessoaRepository;
			_artesanatoRepository = artesanatoRepository;
			_unitOfWork = unitOfWork;
			_amazonS3Service = amazonS3Service;

		}
		#endregion

		#region CRUD
		public async Task<List<ArtesaoDto>> BuscarTodos()
		{
			var artesaos = await _artesaoRepository.BuscarTodos();
			return artesaos.Where(a => a.IsAtivo).Select(ArtesaoFactory.CriarDto).ToList();
		}
		public async Task<ArtesaoDto> BuscarPorId(string id)
		{
			IdValidator.Validar(id);
			var artesao = await _artesaoRepository.BuscarPorId(id);
			if (artesao == null || !artesao.IsAtivo)
				throw new NotFoundException("Artesão não foi encontrado.");

			return ArtesaoFactory.CriarDto(artesao);
		}

		public async Task<ArtesaoDto> Adicionar(ArtesaoDto dto, IFormFile imagem)
		{
			IdValidator.Validar(dto.PessoaId, "PessoaId inválido.");
			var pessoa = await _pessoaRepository.BuscarPorId(dto.PessoaId);
			if (pessoa == null || !pessoa.IsAtivo)
				throw new NotFoundException("Pessoa não foi encontrada.");

			if (pessoa.EhArtesao)
				throw new ConflictException("Pessoa já é artesão.");

			if (imagem != null && imagem.Length > 0)
			{
				ImagemUploadValidator.Validar(imagem);
				using var stream = imagem.OpenReadStream();
				var imagemUrl = await _amazonS3Service.Upload(stream, S3FileHelper.GerarCaminhoArquivo("artesaos", imagem.FileName), imagem.ContentType);
				dto.Foto = imagemUrl;
			}

			var artesao = ArtesaoFactory.CriarEntidade(dto);
			pessoa.TornarArtesao();

			await _unitOfWork.ExecutarAsync(async () =>
			{
				await _artesaoRepository.Adicionar(artesao);
				await _pessoaRepository.Atualizar(pessoa, pessoa.Id);
			});

			return ArtesaoFactory.CriarDto(artesao);
		}
		public async Task<ArtesaoDto> Atualizar(ArtesaoDto dto, string id, IFormFile? imagem = null)
		{
			IdValidator.Validar(id);

			var artesaoExistente = await _artesaoRepository.BuscarPorId(id);

			if (artesaoExistente == null || !artesaoExistente.IsAtivo)
				throw new NotFoundException("Artesão não foi encontrado.");

			dto.PessoaId = artesaoExistente.PessoaId;

			if (imagem != null && imagem.Length > 0)
			{
				ImagemUploadValidator.Validar(imagem);
				using var stream = imagem.OpenReadStream();
				var imagemUrl = await _amazonS3Service.Upload(stream, S3FileHelper.GerarCaminhoArquivo("artesaos", imagem.FileName), imagem.ContentType);
				dto.Foto = imagemUrl;
			}
			else
			{
				dto.Foto = artesaoExistente.Foto;
			}

			var novoArtesao = ArtesaoFactory.CriarEntidade(dto);
			artesaoExistente.Atualizar(
				novoArtesao.Nome,
				novoArtesao.Descricao,
				novoArtesao.Foto,
				novoArtesao.EnderecoComercial,
				novoArtesao.RedesSociais,
				novoArtesao.RecebeEncomenda,
				novoArtesao.EnviaEncomenda,
				novoArtesao.LocalFisico,
				novoArtesao.FeiraMunicipal
			);
			artesaoExistente.AtualizarEspecialidades(novoArtesao.Especialidade);

			await _artesaoRepository.Atualizar(artesaoExistente, id);

			return ArtesaoFactory.CriarDto(artesaoExistente);
		}

		public async Task<bool> Apagar(string id)
		{
			IdValidator.Validar(id);

			var artesao = await _artesaoRepository.BuscarPorId(id);
			if (artesao == null || !artesao.IsAtivo)
				throw new NotFoundException("Artesão não foi encontrado.");

			await _unitOfWork.ExecutarAsync(async () =>
			{
				var artesanatos = await _artesanatoRepository.BuscarTodosPorArtesaoId(id);
				foreach (var artesanato in artesanatos)
				{
					artesanato.Inativar();
					await _artesanatoRepository.Atualizar(artesanato, artesanato.Id);
				}

				artesao.Inativar();
				await _artesaoRepository.Atualizar(artesao, id);

				var pessoa = await _pessoaRepository.BuscarPorId(artesao.PessoaId);
				if (pessoa != null)
				{
					pessoa.RemoverArtesao();
					await _pessoaRepository.Atualizar(pessoa, pessoa.Id);
				}
			});
			return true;
		}
		#endregion


		public async Task<List<ArtesaoDto>> BuscarComFiltro(string? nome, string? especialidade, bool? recebeEncomenda, bool? enviaEncomenda)
		{
			var filtros = new List<FilterDefinition<Artesao>> { Builders<Artesao>.Filter.Eq(a => a.IsAtivo, true) };

			if (!string.IsNullOrWhiteSpace(nome))
				filtros.Add(Builders<Artesao>.Filter.Regex(a => a.Nome, new BsonRegularExpression(nome, "i")));

			if (!string.IsNullOrWhiteSpace(especialidade))
				filtros.Add(Builders<Artesao>.Filter.Regex(a => a.Especialidade, new BsonRegularExpression(especialidade, "i")));

			if (recebeEncomenda.HasValue)
				filtros.Add(Builders<Artesao>.Filter.Eq(a => a.RecebeEncomenda, recebeEncomenda.Value));

			if (enviaEncomenda.HasValue)
				filtros.Add(Builders<Artesao>.Filter.Eq(a => a.EnviaEncomenda, enviaEncomenda.Value));

			FilterDefinition<Artesao> filtroFinal = filtros.Count > 0
				? Builders<Artesao>.Filter.And(filtros)
				: Builders<Artesao>.Filter.Empty;

			var artesaos = await _artesaoRepository.BuscarComFiltro(filtroFinal);
			return artesaos.Select(ArtesaoFactory.CriarDto).ToList();
		}


	}
}
