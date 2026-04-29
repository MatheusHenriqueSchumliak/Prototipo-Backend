using PrototipoBackEnd.Application.Dtos.Artesao;
using PrototipoBackEnd.Domain.Entities;

namespace PrototipoBackEnd.Application.Factories
{
	public static class ArtesaoFactory
	{
		public static ArtesaoDto CriarDto(Artesao artesao)
		{
			return new ArtesaoDto
			{
				Id = artesao.Id,
				PessoaId = artesao.PessoaId,
				Nome = artesao.Nome,
				Descricao = artesao.Descricao,
				Foto = artesao.Foto,
				RecebeEncomenda = artesao.RecebeEncomenda,
				EnviaEncomenda = artesao.EnviaEncomenda,
				LocalFisico = artesao.LocalFisico,
				FeiraMunicipal = artesao.FeiraMunicipal,
				Especialidade = EspecialidadeFactory.CriarDto(artesao.Especialidade),
				Endereco = artesao.EnderecoComercial != null ? EnderecoFactory.CriarDto(artesao.EnderecoComercial) : null,
				RedesSociais = RedesSociaisFactory.CriarDto(artesao.RedesSociais),
				DataCriacao = artesao.DataCriacao,
				DataAtualizacao = artesao.DataAtualizacao,
				DataRemocao = artesao.DataRemocao,
				IsAtivo = artesao.IsAtivo
			};
		}

		public static Artesao CriarEntidade(ArtesaoDto dto)
		{
			return Artesao.Criar(
				dto.PessoaId,
				dto.Nome,
				dto.Descricao,
				dto.Foto,
				EspecialidadeFactory.CriarEntidade(dto.Especialidade),
				dto.Endereco != null ? EnderecoFactory.CriarEntidade(dto.Endereco) : null,
				RedesSociaisFactory.CriarEntidade(dto.RedesSociais),
				dto.RecebeEncomenda,
				dto.EnviaEncomenda,
				dto.LocalFisico,
				dto.FeiraMunicipal
			);
		}
	}
}
