using PrototipoBackEnd.Application.Dtos.Artesanato;
using PrototipoBackEnd.Domain.Entities;

namespace PrototipoBackEnd.Application.Factories
{
	public static class ArtesanatoFactory
	{
		public static ArtesanatoDto CriarDto(Artesanato artesanato)
		{
			return new ArtesanatoDto
			{
				Id = artesanato.Id,
				PessoaId = artesanato.PessoaId,
				ArtesaoId = artesanato.ArtesaoId,
				Titulo = artesanato.Titulo,
				Descricao = artesanato.Descricao,
				TempoCriacao = artesanato.TempoCriacao,
				Quantidade = artesanato.Quantidade,
				Preco = artesanato.Preco,
				TemEstoque = artesanato.TemEstoque,
				SomenteEncomenda = artesanato.SomenteEncomenda,
				AceitaEncomenda = artesanato.AceitaEncomenda,
				Categoria = CategoriaFactory.CriarDto(artesanato.Categoria),
				Material = MaterialFactory.CriarDto(artesanato.Material),
				Midia = MidiaFactory.CriarDto(artesanato.Midia),
				DataCriacao = artesanato.DataCriacao,
				DataAtualizacao = artesanato.DataAtualizacao,
				DataRemocao = artesanato.DataRemocao,
				IsAtivo = artesanato.IsAtivo
			};
		}

		public static Artesanato CriarEntidade(ArtesanatoDto dto)
		{
			return Artesanato.Criar(
				dto.PessoaId,
				dto.ArtesaoId,
				dto.Titulo,
				dto.Descricao,
				dto.TempoCriacao,
				dto.Quantidade,
				dto.Preco,
				dto.TemEstoque,
				dto.SomenteEncomenda,
				dto.AceitaEncomenda,
				CategoriaFactory.CriarEntidade(dto.Categoria),
				MaterialFactory.CriarEntidade(dto.Material),
				MidiaFactory.CriarEntidade(dto.Midia)
			);
		}
	}
}
