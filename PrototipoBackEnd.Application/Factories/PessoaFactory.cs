using PrototipoBackEnd.Application.Dtos.Pessoa;
using PrototipoBackEnd.Domain.Entities;

namespace PrototipoBackEnd.Application.Factories
{
	public static class PessoaFactory
	{
		public static PessoaDto CriarDto(Pessoa pessoa)
		{
			return new PessoaDto
			{
				Id = pessoa.Id,
				NomeCompleto = pessoa.NomeCompleto,
				DataNascimento = pessoa.DataNascimento,
				Endereco = EnderecoFactory.CriarDto(pessoa.Endereco),
				Contato = ContatoFactory.CriarDto(pessoa.Contato),
				TemUsuario = pessoa.TemUsuario,
				EhArtesao = pessoa.EhArtesao,
				DataCriacao = pessoa.DataCriacao,
				DataAtualizacao = pessoa.DataAtualizacao,
				DataRemocao = pessoa.DataRemocao,
				IsAtivo = pessoa.IsAtivo
			};
		}

		public static Pessoa CriarEntidade(PessoaDto dto)
		{
			return Pessoa.Criar(
				dto.NomeCompleto,
				dto.DataNascimento,
				EnderecoFactory.CriarEntidade(dto.Endereco),
				ContatoFactory.CriarEntidade(dto.Contato)
			);
		}
	}
}
