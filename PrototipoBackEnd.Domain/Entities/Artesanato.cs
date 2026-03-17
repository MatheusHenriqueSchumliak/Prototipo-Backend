using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace PrototipoBackEnd.Domain.Entities;

public class Artesanato
{
	[BsonId]
	[BsonRepresentation(BsonType.String)]
	public required string Id { get; set; }
	[BsonRepresentation(BsonType.String)]
	public required string UsuarioId { get; set; }
	public string? ArtesaoId { get; set; } = null!;
	public List<string>? ImagemUrl { get; set; } = null!;
	public bool? SobEncomenda { get; set; } = null!;
	public bool? AceitaEncomenda { get; set; } = null!;
	public List<string>? CategoriaTags { get; set; } = null!;
	public string? TituloArtesanato { get; set; } = null!;
	public decimal? Preco { get; set; } = null!;
	public int QuantidadeArtesanato { get; set; }
	public string? DescricaoArtesanato { get; set; } = null!;
	public string? MateriaisUtilizados { get; set; } = null!;
	public DateTime? DataCriacao { get; set; } = null!;
	public int? TempoCriacaoHr { get; set; } = null!;

	[BsonIgnore]
	public Artesao? Artesao { get; set; }

	public Artesanato() { }

	#region Metodos
	public static Artesanato Criar(
	string id,
	string usuarioId,
	string? artesaoId,
	IEnumerable<string>? imagemUrl,
	bool? sobEncomenda,
	bool? aceitaEncomenda,
	IEnumerable<string>? categoriaTags,
	string? tituloArtesanato,
	decimal? preco,
	int quantidadeArtesanato,
	string? descricaoArtesanato,
	string? materiaisUtilizados,
	DateTime? dataCriacao,
	int? tempoCriacaoHr,
	Artesao? artesao = null)
	{
		// Id / UsuarioId obrigatórios
		if (string.IsNullOrWhiteSpace(id))
			throw new ArgumentException("Id vazio.");
		if (string.IsNullOrWhiteSpace(usuarioId))
			throw new ArgumentException("UsuarioId vazio.");
		id = id.Trim();
		usuarioId = usuarioId.Trim();

		// ArtesaoId opcional
		if (!string.IsNullOrWhiteSpace(artesaoId))
			artesaoId = artesaoId.Trim();
		else
			artesaoId = null;

		// Imagens: se informadas, validar URIs e normalizar
		List<string>? imagemList = null;
		if (imagemUrl != null)
		{
			imagemList = imagemUrl
				.Where(s => !string.IsNullOrWhiteSpace(s))
				.Select(s => s!.Trim())
				.ToList();

			if (imagemList.Count == 0)
				imagemList = null;
			else
			{
				if (imagemList.Count > 20)
					throw new ArgumentException("ImagemUrl: máximo de 20 imagens permitido.");

				foreach (var img in imagemList)
				{
					if (!Uri.TryCreate(img, UriKind.Absolute, out var u) || (u.Scheme != Uri.UriSchemeHttp && u.Scheme != Uri.UriSchemeHttps))
						throw new ArgumentException("ImagemUrl inválida.");
				}
			}
		}

		// SobEncomenda / AceitaEncomenda: nada a validar além de tipo, mas manter coerência básica
		// (se fornecido, manter como está)

		// CategoriaTags: normalizar, sem vazios, sem duplicatas, limites
		List<string>? tags = null;
		if (categoriaTags != null)
		{
			tags = categoriaTags
				.Where(t => !string.IsNullOrWhiteSpace(t))
				.Select(t => t!.Trim())
				.Where(t => t.Length > 0)
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.ToList();

			if (tags.Count == 0)
				tags = null;
			else
			{
				if (tags.Count > 50) throw new ArgumentException("CategoriaTags: máximo de 50 tags permitido.");
				foreach (var tag in tags)
				{
					if (tag.Length > 50) throw new ArgumentException("CategoriaTags contém tag muito longa.");
				}
			}
		}

		// Título: opcional, trim e limite
		if (!string.IsNullOrWhiteSpace(tituloArtesanato))
		{
			tituloArtesanato = tituloArtesanato.Trim();
			if (tituloArtesanato.Length > 200) throw new ArgumentException("TituloArtesanato muito longo.");
		}
		else
		{
			tituloArtesanato = null;
		}

		// Preço: se informado >= 0
		if (preco.HasValue)
		{
			if (preco.Value < 0m) throw new ArgumentOutOfRangeException(nameof(preco), "Preco inválido.");
		}

		// Quantidade: >= 0
		if (quantidadeArtesanato < 0) throw new ArgumentOutOfRangeException(nameof(quantidadeArtesanato), "QuantidadeArtesanato inválida.");

		// Descrição: opcional, trim e limite
		if (!string.IsNullOrWhiteSpace(descricaoArtesanato))
		{
			descricaoArtesanato = descricaoArtesanato.Trim();
			if (descricaoArtesanato.Length > 2000) throw new ArgumentException("DescricaoArtesanato muito longa.");
		}
		else
		{
			descricaoArtesanato = null;
		}

		// Materiais: opcional, trim e limite
		if (!string.IsNullOrWhiteSpace(materiaisUtilizados))
		{
			materiaisUtilizados = materiaisUtilizados.Trim();
			if (materiaisUtilizados.Length > 1000) throw new ArgumentException("MateriaisUtilizados muito longo.");
		}
		else
		{
			materiaisUtilizados = null;
		}

		// Data de criação: se informada, não pode ser no futuro (pequeno buffer)
		if (dataCriacao.HasValue)
		{
			if (dataCriacao.Value.ToUniversalTime() > DateTime.UtcNow.AddMinutes(1))
				throw new ArgumentException("DataCriacao não pode ser no futuro.");
		}

		// Tempo de criação (horas): se informado, >= 0 e <= 10000
		if (tempoCriacaoHr.HasValue)
		{
			if (tempoCriacaoHr.Value < 0 || tempoCriacaoHr.Value > 10000)
				throw new ArgumentOutOfRangeException(nameof(tempoCriacaoHr), "TempoCriacaoHr inválido.");
		}

		// Coerência entre artesaoId e objeto Artesao, se ambos fornecidos
		if (!string.IsNullOrWhiteSpace(artesaoId) && artesao != null)
		{
			if (!string.Equals(artesaoId, artesao.Id, StringComparison.OrdinalIgnoreCase))
				throw new ArgumentException("ArtesaoId e Artesao.Id divergem.");
		}

		// Monta e retorna
		return new Artesanato
		{
			Id = id,
			UsuarioId = usuarioId,
			ArtesaoId = artesaoId,
			ImagemUrl = imagemList,
			SobEncomenda = sobEncomenda,
			AceitaEncomenda = aceitaEncomenda,
			CategoriaTags = tags,
			TituloArtesanato = tituloArtesanato,
			Preco = preco,
			QuantidadeArtesanato = quantidadeArtesanato,
			DescricaoArtesanato = descricaoArtesanato,
			MateriaisUtilizados = materiaisUtilizados,
			DataCriacao = dataCriacao,
			TempoCriacaoHr = tempoCriacaoHr,
			Artesao = artesao
		};
	}


	#endregion

}