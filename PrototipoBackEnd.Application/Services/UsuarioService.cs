using PrototipoBackEnd.Application.Common.Extensions;
using PrototipoBackEnd.Application.Interfaces;
using PrototipoBackEnd.Domain.Enumerables;
using PrototipoBackEnd.Domain.Interfaces;
using PrototipoBackEnd.Application.Dtos;
using PrototipoBackEnd.Domain.Entities;
using AutoMapper;

namespace PrototipoBackEnd.Application.Services
{
	public class UsuarioService : IUsuarioService
	{
		#region Construtor
		private readonly IUsuarioRepository _usuarioRepository;
		private readonly ISenhaService _senhaService;
		private readonly IMapper _mapper;

		public UsuarioService(IUsuarioRepository usuarioRepository, ISenhaService senhaService, IMapper mapper)
		{
			_usuarioRepository = usuarioRepository;
			_senhaService = senhaService;
			_mapper = mapper;
		}
		#endregion

		public async Task<List<UsuarioDto>> BuscarTodos()
		{
			var usuarios = await _usuarioRepository.BuscarTodos();
			return _mapper.Map<List<UsuarioDto>>(usuarios);
		}

		public async Task<UsuarioDto> BuscarPorId(string id)
		{
			try
			{
				// Converte o ID para GUID, já que seu banco está usando GUID para o _id
				var usuario = await _usuarioRepository.BuscarPorId(id);

				// Se o usuário for encontrado, retorna o DTO
				if (usuario != null)
				{
					return _mapper.Map<UsuarioDto>(usuario);
				}

				throw new Exception($"Usuário não foi encontrado!"); // Retorna null caso o usuário não seja encontrado
			}
			catch (Exception ex)
			{
				// Em caso de erro, lançar a exceção para ser tratada na controller
				throw new Exception($"Erro ao buscar usuário: {ex.Message}");
			}
		}

		public async Task Adicionar(UsuarioDto dto)
		{
			try
			{
				var hash = _senhaService.CriarHash(dto.SenhaHash);
				bool ativo = true;
				var usuario = Usuario.Criar(dto.Nome, dto.Email, hash, UsuarioEnum.Administrador, ativo);

				await _usuarioRepository.Adicionar(usuario);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Erro ao adicionar Usuário: {ex.Message}");
			}
		}

		public async Task Atualizar(UsuarioDto dto, string id)
		{
			if (id == null)
				throw new Exception($"Usuário para o ID: {id} não foi encontrado!");

			var usuarioExistente = await _usuarioRepository.BuscarPorId(id);

			if (usuarioExistente == null)
				throw new Exception($"Usuário com o ID: {id} não foi encontrado!");

			// Atualiza a senha, se necessário
			AtualizarSenha(usuarioExistente, dto);

			// Usa o AutoMapper para atualizar as propriedades do usuário  
			var novoUsuario = _mapper.Map<Usuario>(dto);
			usuarioExistente.ApplyIfChanged(novoUsuario);

			// Persiste as alterações  
			await _usuarioRepository.Atualizar(usuarioExistente, id); // Corrigido para passar apenas o objeto atualizado  
		}

		public async Task<bool> Apagar(string id)
		{
			if (id == null)
			{
				throw new Exception($"Usuário para o ID: {id} não foi encontrado!");
			}

			await _usuarioRepository.Deletar(id);
			return true;
		}

		#region Métodos Privados
		// Método responsável por atualizar a senha
		private void AtualizarSenha(Usuario usuarioExistente, UsuarioDto dto)
		{
			if (!string.IsNullOrWhiteSpace(dto.SenhaHash) && dto.SenhaHash != usuarioExistente.SenhaHash)
			{
				dto.SenhaHash = _senhaService.CriarHash(dto.SenhaHash);
			}
		}
		#endregion

	}
}
