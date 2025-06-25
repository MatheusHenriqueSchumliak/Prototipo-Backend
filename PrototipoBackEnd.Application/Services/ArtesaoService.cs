using PrototipoBackEnd.Domain.Interfaces.Repositories;
using PrototipoBackEnd.Domain.Interfaces.Services;
using PrototipoBackEnd.Application.Interfaces;
using PrototipoBackEnd.Application.Dtos;
using PrototipoBackEnd.Domain.Entities;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using MongoDB.Bson;
using AutoMapper;
using static System.Net.Mime.MediaTypeNames;

namespace PrototipoBackEnd.Application.Services
{
	public class ArtesaoService : IArtesaoService
	{
		#region Construtor
		private readonly IArtesaoRepository _artesaoRepository;
		private readonly IAmazonS3Service _amazonS3Service;
		private readonly IMapper _mapper;
		public ArtesaoService(IArtesaoRepository artesaoRepository, IAmazonS3Service amazonS3Service, IMapper mapper)
		{
			_artesaoRepository = artesaoRepository;
			_amazonS3Service = amazonS3Service;
			_mapper = mapper;
		}
		#endregion

		#region CRUD
		public async Task<List<ArtesaoDto>> BuscarTodos()
		{
			var artesaos = await _artesaoRepository.BuscarTodos();
			return _mapper.Map<List<ArtesaoDto>>(artesaos);
		}
		public async Task<ArtesaoDto> BuscarPorId(string id)
		{
			try
			{
				var artesao = await _artesaoRepository.BuscarPorId(id);
				if (artesao == null)
					throw new Exception($"Artesão não foi encontrado!");

				var dto = _mapper.Map<ArtesaoDto>(artesao);

				//// Recupera a imagem se tiver FotoUrl
				//if (!string.IsNullOrEmpty(dto.FotoUrl))
				//{
				//	var fileName = Path.GetFileName(dto.FotoUrl); // Extrai o nome do arquivo da URL
				//	var imageStream = await _amazonS3Service.GetFile(fileName);

				//	if (imageStream != null)
				//	{
				//		using var ms = new MemoryStream();
				//		await imageStream.CopyToAsync(ms);
				//		dto.FotoUrl = ms.ToArray(); // Supondo que você tenha essa propriedade
				//	}
				//}

				return dto;
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
				dto.FotoUrl = imagemUrl;
			}

			try
			{
				var artesao = Artesao.Criar(
					dto.UsuarioId,
					dto.NomeCompleto,
					dto.Idade,
					dto.NomeArtesao,
					dto.Telefone,
					dto.WhatsApp,
					dto.Email,
					dto.Instagram,
					dto.Facebook,
					dto.DescricaoPerfil,
					dto.ReceberEncomendas,
					dto.EnviaEncomendas,
					dto.FotoUrl,
					dto.NichoAtuacao,
					dto.LocalFisico,
					dto.FeiraMunicipal,
					dto.CEP,
					dto.Estado,
					dto.Cidade,
					dto.Rua,
					dto.Bairro,
					dto.Complemento,
					dto.Numero,
					dto.SemNumero
				);

				await _artesaoRepository.Adicionar(artesao);

				return _mapper.Map<ArtesaoDto>(artesao);
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

			try
			{
				// ✅ Se uma nova imagem foi enviada, fazer upload
				if (imagem != null && imagem.Length > 0)
				{
					using var stream = imagem.OpenReadStream();
					var imagemUrl = await _amazonS3Service.Upload(stream, imagem.FileName, imagem.ContentType);
					dto.FotoUrl = imagemUrl;
				}
				else
				{
					// ✅ Manter a imagem existente se não foi enviada nova
					dto.FotoUrl = artesaoExistente.FotoUrl;
				}

				// ✅ Mapear os dados do DTO para a entidade existente
				_mapper.Map(dto, artesaoExistente);

				// ✅ Manter o ID original (importante!)
				artesaoExistente.Id = id;

				// ✅ Salvar as alterações no banco
				await _artesaoRepository.Atualizar(artesaoExistente, id);

				// ✅ Retornar o DTO atualizado
				return _mapper.Map<ArtesaoDto>(artesaoExistente);
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


		public async Task<List<ArtesaoDto>> BuscarComFiltro(string? nome, string? nichoAtuacao, bool? receberEncomendas, bool? enviaEncomendas)
		{
			var filtros = new List<FilterDefinition<Artesao>>();

			if (!string.IsNullOrWhiteSpace(nome))
				filtros.Add(Builders<Artesao>.Filter.Regex(a => a.NomeArtesao, new BsonRegularExpression(nome, "i")));

			if (!string.IsNullOrWhiteSpace(nichoAtuacao))
				filtros.Add(Builders<Artesao>.Filter.Regex(a => a.NichoAtuacao, new BsonRegularExpression(nichoAtuacao, "i")));

			if (receberEncomendas.HasValue)
				filtros.Add(Builders<Artesao>.Filter.Eq(a => a.ReceberEncomendas, receberEncomendas.Value));

			if (enviaEncomendas.HasValue)
				filtros.Add(Builders<Artesao>.Filter.Eq(a => a.EnviaEncomendas, enviaEncomendas.Value));

			FilterDefinition<Artesao> filtroFinal = filtros.Any()
				? Builders<Artesao>.Filter.And(filtros)
				: Builders<Artesao>.Filter.Empty;

			var artesaos = await _artesaoRepository.BuscarComFiltro(filtroFinal);
			return _mapper.Map<List<ArtesaoDto>>(artesaos);
		}


	}
}
