using PrototipoBackEnd.Domain.Entities.Base;
using PrototipoBackEnd.Domain.ValueObjects;

namespace PrototipoBackEnd.Domain.Entities
{
	public class Artesanato : EntityBase
	{
		public string PessoaId { get; private set; }
		public string ArtesaoId { get; private set; }
		public string Titulo { get; private set; }
		public string Descricao { get; private set; }
		public int TempoCriacao { get; private set; }
		public int Quantidade { get; private set; }
		public decimal? Preco { get; private set; }
		public bool TemEstoque { get; private set; }
		public bool SomenteEncomenda { get; private set; }
		public bool AceitaEncomenda { get; private set; }
		public Categoria Categoria { get; private set; }
		public Material Material { get; private set; }
		public Midia Midia { get; private set; }

		public Artesanato() { }

		public static Artesanato Criar(string pessoaId, string artesaoId, string titulo, string descricao, int tempoCriacao, int quantidade, decimal? preco, bool temEstoque, bool somenteEncomenda, bool aceitaEncomenda, Categoria categoria, Material material, Midia midia)
		{
			if (string.IsNullOrWhiteSpace(pessoaId))
				throw new ArgumentException("PessoaId vazio.");

			if (string.IsNullOrWhiteSpace(artesaoId))
				throw new ArgumentException("ArtesaoId vazio.");

			if (string.IsNullOrWhiteSpace(titulo))
				throw new ArgumentException("Título obrigatório.");

			if (tempoCriacao < 0)
				throw new ArgumentException("Tempo de criação inválido.");

			if (temEstoque && quantidade <= 0)
				throw new ArgumentException("Quantidade deve ser maior que zero quando há estoque.");

			if (!temEstoque && quantidade > 0)
				throw new ArgumentException("Quantidade não deve ser informada quando não há estoque.");

			if (somenteEncomenda && temEstoque)
				throw new ArgumentException("Não pode ter estoque e ser somente encomenda.");

			return new Artesanato
			{
				Id = Guid.NewGuid().ToString(),
				PessoaId = pessoaId,
				ArtesaoId = artesaoId,
				Titulo = titulo.Trim(),
				Descricao = descricao.Trim(),
				TempoCriacao = tempoCriacao,
				Quantidade = quantidade,
				Preco = preco,
				TemEstoque = temEstoque,
				SomenteEncomenda = somenteEncomenda,
				AceitaEncomenda = aceitaEncomenda,
				Categoria = categoria,
				Material = material,
				Midia = midia,
				DataCriacao = DateTime.UtcNow
			};
		}

		public void Atualizar(string? titulo, string? descricao, int? tempoCriacao, int? quantidade, decimal? preco, bool? temEstoque, bool? somenteEncomenda, bool? aceitaEncomenda, Categoria? categoria, Material? material, Midia? midia)
		{
			if (!string.IsNullOrWhiteSpace(titulo))
				Titulo = titulo.Trim();

			if (!string.IsNullOrWhiteSpace(descricao))
				Descricao = descricao.Trim();

			if (tempoCriacao.HasValue)
			{
				if (tempoCriacao.Value < 0)
					throw new ArgumentException("Tempo inválido.");
				TempoCriacao = tempoCriacao.Value;
			}

			if (temEstoque.HasValue)
				TemEstoque = temEstoque.Value;

			if (quantidade.HasValue)
				Quantidade = quantidade.Value;

			if (TemEstoque && Quantidade <= 0)
				throw new ArgumentException("Quantidade obrigatória quando há estoque.");

			if (!TemEstoque && Quantidade > 0)
				throw new ArgumentException("Quantidade inválida sem estoque.");

			if (somenteEncomenda.HasValue)
				SomenteEncomenda = somenteEncomenda.Value;

			if (aceitaEncomenda.HasValue)
				AceitaEncomenda = aceitaEncomenda.Value;

			if (SomenteEncomenda && TemEstoque)
				throw new ArgumentException("Conflito: estoque + somente encomenda.");

			if (preco.HasValue)
			{
				if (preco.Value < 0)
					throw new ArgumentException("Preço inválido.");
				Preco = preco;
			}

			if (categoria != null)
				Categoria = categoria;

			if (material != null)
				Material = material;

			if (midia != null)
				Midia = midia;

			MarcarAtualizado(); // do EntityBase
		}

		public void Inativar()
		{
			if (DataRemocao.HasValue)
				return;

			Quantidade = 0;
			TemEstoque = false;
			AceitaEncomenda = false;

			Remover(); // do EntityBase
		}

		public void Reativar()
		{
			DataRemocao = null;
			MarcarAtualizado(); // do EntityBase
		}

	}
}
