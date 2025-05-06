using PrototipoBackEnd.Domain.Interfaces.Repositories;
using PrototipoBackEnd.Application.Interfaces;
using PrototipoBackEnd.Application.Dtos;
using PrototipoBackEnd.Domain.Entities;
using AutoMapper;

namespace PrototipoBackEnd.Application.Services
{
	public class ArtesaoService : IArtesaoService
	{
		#region Construtor
		private readonly IArtesaoRepository _artesaoRepository;
		private readonly IMapper _mapper;
		public ArtesaoService(IArtesaoRepository artesaoRepository, IMapper mapper)
		{
			_artesaoRepository = artesaoRepository;
			_mapper = mapper;
		}
		#endregion

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
				if (artesao != null)
					return _mapper.Map<ArtesaoDto>(artesao);

				throw new Exception($"Artesão não foi encontrado!");
			}
			catch (Exception)
			{
				throw new Exception($"Artesão não foi encontrado!");
			}
		}
		public async Task Adicionar(ArtesaoDto dto)
		{
			try
			{				
				var artesao = Artesao.Criar(
					dto.UsuarioId,
					dto.NomeArtesao,
					dto.Telefone,
					dto.WhatsApp,
					dto.DescricaoPerfil,
					dto.ReceberEncomendas,
					dto.EnviaEncomendas,
					dto.FotoUrl,
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
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Erro ao adicionar Artesão: {ex.Message}");
			}
		}
		public async Task Atualizar(ArtesaoDto dto, string id)
		{
			if (id == null)
				throw new Exception($"Artesão para o ID: {id} não foi encontrado!");

			var artesaoExistente = await _artesaoRepository.BuscarPorId(id);

			if (artesaoExistente == null)
				throw new Exception($"Artesão com o ID: {id} não foi encontrado!");
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

	}
}
