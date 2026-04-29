using PrototipoBackEnd.Domain.Interfaces.Repositories;
using PrototipoBackEnd.Application.Dtos.Pessoa;
using PrototipoBackEnd.Application.Interfaces;
using PrototipoBackEnd.Application.Factories;

namespace PrototipoBackEnd.Application.Services
{
	public class PessoaService : IPessoaService
	{
		private readonly IPessoaRepository _pessoaRepository;

		public PessoaService(IPessoaRepository pessoaRepository)
		{
			_pessoaRepository = pessoaRepository;
		}

		public async Task<List<PessoaDto>> BuscarTodos()
		{
			var pessoas = await _pessoaRepository.BuscarTodos();
			return pessoas.Select(PessoaFactory.CriarDto).ToList();
		}

		public async Task<PessoaDto> BuscarPorId(string id)
		{
			var pessoa = await _pessoaRepository.BuscarPorId(id);
			if (pessoa == null)
				throw new Exception("Pessoa não encontrada!");
			return PessoaFactory.CriarDto(pessoa);
		}

		public async Task<PessoaDto> Adicionar(PessoaDto dto)
		{
			var pessoa = PessoaFactory.CriarEntidade(dto);
			var adicionada = await _pessoaRepository.Adicionar(pessoa);
			return PessoaFactory.CriarDto(adicionada);
		}

		public async Task<PessoaDto> Atualizar(PessoaDto dto, string id)
		{
			var pessoaExistente = await _pessoaRepository.BuscarPorId(id);
			if (pessoaExistente == null)
				throw new Exception("Pessoa não encontrada!");

			pessoaExistente.Atualizar(
				dto.NomeCompleto,
				dto.DataNascimento,
				EnderecoFactory.CriarEntidade(dto.Endereco),
				ContatoFactory.CriarEntidade(dto.Contato)
			);

			var atualizada = await _pessoaRepository.Atualizar(pessoaExistente, id);
			return PessoaFactory.CriarDto(atualizada);
		}

		public async Task<bool> Apagar(string id)
		{
			var pessoa = await _pessoaRepository.BuscarPorId(id);
			if (pessoa == null)
				throw new Exception("Pessoa não encontrada!");

			await _pessoaRepository.Deletar(id);
			return true;
		}

	}
}
