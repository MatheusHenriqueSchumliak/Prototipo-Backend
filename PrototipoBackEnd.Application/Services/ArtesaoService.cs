using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using PrototipoBackEnd.Application.Dtos.Artesao;
using PrototipoBackEnd.Application.Factories;
using PrototipoBackEnd.Application.Interfaces;
using PrototipoBackEnd.Domain.Entities;
using PrototipoBackEnd.Domain.Interfaces.Repositories;
using PrototipoBackEnd.Domain.Interfaces.Services;
using PrototipoBackEnd.Domain.ValueObjects;

namespace PrototipoBackEnd.Application.Services
{
	public class ArtesaoService : IArtesaoService
	{
		#region Construtor
		private readonly IArtesaoRepository _artesaoRepository;
		private readonly IAmazonS3Service _amazonS3Service;
		public ArtesaoService(IArtesaoRepository artesaoRepository, IAmazonS3Service amazonS3Service)
		{
			_artesaoRepository = artesaoRepository;
			_amazonS3Service = amazonS3Service;

		}
		#endregion

		#region CRUD
		public async Task<List<ArtesaoDto>> BuscarTodos()
		{
			var artesaos = await _artesaoRepository.BuscarTodos();
			return artesaos.Select(ArtesaoFactory.CriarDto).ToList();
		}
		public async Task<ArtesaoDto> BuscarPorId(string id)
		{
			try
			{
				var artesao = await _artesaoRepository.BuscarPorId(id);
				if (artesao == null)
					throw new Exception($"Artesão não foi encontrado!");

				return ArtesaoFactory.CriarDto(artesao);
			}
			catch (Exception)
			{
				throw new Exception($"Artesão não foi encontrado!");
			}
		}

		public async Task<ArtesaoDto> Adicionar(ArtesaoDto dto, IFormFile imagem)
		{
			if (imagem != null && imagem.Length > 0)
			{
				using var stream = imagem.OpenReadStream();
				var imagemUrl = await _amazonS3Service.Upload(stream, imagem.FileName, imagem.ContentType);
				dto.Foto = imagemUrl;
			}

			try
			{
				var artesao = ArtesaoFactory.CriarEntidade(dto);

				await _artesaoRepository.Adicionar(artesao);

				return ArtesaoFactory.CriarDto(artesao);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Erro ao adicionar Artesão: {ex.Message}");
				throw; // Para o controller tratar
			}
		}
		public async Task<ArtesaoDto> Atualizar(ArtesaoDto dto, string id, IFormFile? imagem = null)
		{
			if (id == null)
				throw new Exception($"Artesão para o ID: {id} não foi encontrado!");

			var artesaoExistente = await _artesaoRepository.BuscarPorId(id);

			if (artesaoExistente == null)
				throw new Exception($"Artesão com o ID: {id} não foi encontrado!");

			if (artesaoExistente.Id != id)
				throw new Exception("O ID do artesão não pode ser alterado.");

			try
			{
				// ✅ Se uma nova imagem foi enviada, fazer upload
				if (imagem != null && imagem.Length > 0)
				{
					using var stream = imagem.OpenReadStream();
					var imagemUrl = await _amazonS3Service.Upload(stream, imagem.FileName, imagem.ContentType);
					dto.Foto = imagemUrl;
				}
				else
				{
					// ✅ Manter a imagem existente se não foi enviada nova
					dto.Foto = artesaoExistente.Foto;
				}

				// Atualiza os campos do artesaoExistente com base no DTO
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
			catch (Exception ex)
			{
				Console.WriteLine($"Erro ao atualizar Artesão: {ex.Message}");
				throw; // Para o controller tratar
			}
		}

		public async Task<bool> Apagar(string id)
		{
			if (id == null)
			{
				throw new Exception($"Artesão para o ID: {id} não foi encontrado!");
			}

			await _artesaoRepository.Deletar(id);
			return true;
		}
		#endregion


		public async Task<List<ArtesaoDto>> BuscarComFiltro(string? nome, string? especialidade, bool? recebeEncomenda, bool? enviaEncomenda)
		{
			var filtros = new List<FilterDefinition<Artesao>>();

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
