using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Net.Mail;

namespace PrototipoBackEnd.Domain.Entities
{
	public class Artesao
	{
		[BsonId]
		[BsonRepresentation(BsonType.String)]
		public string Id { get; set; }

		[BsonRepresentation(BsonType.String)]
		public required string UsuarioId { get; set; }

		public string NomeCompleto { get; set; } = null!;
		public int Idade { get; set; }
		public string NomeArtesao { get; set; } = null!;
		public string Telefone { get; set; } = null!;
		public string WhatsApp { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string Instagram { get; set; } = null!;
		public string Facebook { get; set; } = null!;
		public string DescricaoPerfil { get; set; } = null!;
		public bool ReceberEncomendas { get; set; } = false;
		public bool EnviaEncomendas { get; set; } = false;
		public string? FotoUrl { get; set; } = null!;
		public string? NichoAtuacao { get; set; } = null!;
		public bool LocalFisico { get; set; } = false;
		public bool FeiraMunicipal { get; set; } = false;
		// Endereço
		public string CEP { get; set; } = null!;
		public string Estado { get; set; } = null!;
		public string Cidade { get; set; } = null!;
		public string Rua { get; set; } = null!;
		public string Bairro { get; set; } = null!;
		public string Complemento { get; set; } = null!;
		public string Numero { get; set; } = null!;
		public bool SemNumero { get; set; } = false;

		// Relacionamento com artesanatos
		public List<string> ArtesanatoIds { get; set; } = new(); // Referência a IDs de artesanatos
		[BsonIgnore]
		public List<Artesanato>? Artesanatos { get; set; }

		#region Metodos 
		public Artesao() { }

		public static Artesao Criar(
			string usuarioId,
			string nomeArtesao,
			int idade,
			string nomeCompleto,
			string telefone,
			string whatsapp,
			string email,
			string instagram,
			string facebook,
			string descricaoPerfil,
			bool receberEncomendas,
			bool enviaEncomendas,
			string fotoUrl,
			string nichoAtuacao,
			bool localFisico,
			bool feiraMunicipal,
			string cep,
			string estado,
			string cidade,
			string rua,
			string bairro,
			string complemento,
			string numero,
			bool semNumero)
		{
			// UsuarioId
			if (string.IsNullOrWhiteSpace(usuarioId))
				throw new ArgumentException("UsuarioId vazio.");

			// Nome Completo
			if (string.IsNullOrWhiteSpace(nomeCompleto))
				throw new ArgumentException("NomeCompleto vazio.");
			nomeCompleto = nomeCompleto.Trim();

			// Nome do Artesão
			if (string.IsNullOrWhiteSpace(nomeArtesao))
				throw new ArgumentException("NomeArtesao vazio.");
			nomeArtesao = nomeArtesao.Trim();

			// Idade: sem valores negativos e razoável (0..120)
			if (idade < 0 || idade > 120)
				throw new ArgumentOutOfRangeException(nameof(idade), "Idade inválida.");

			// dentro do método Criar, antes de validação/return
			string NormalizePhone(string p)
			{
				if (string.IsNullOrWhiteSpace(p)) return string.Empty;
				return new string(p.Where(char.IsDigit).ToArray());
			}

			var telefoneDigits = NormalizePhone(telefone);
			var whatsappDigits = NormalizePhone(whatsapp);

			if (string.IsNullOrWhiteSpace(telefoneDigits) || telefoneDigits.Length < 8)
				throw new ArgumentException("Telefone inválido.");

			// Corrigido: lançar exceção quando WhatsApp for inválido
			if (string.IsNullOrWhiteSpace(whatsappDigits) || whatsappDigits.Length < 8)
				throw new ArgumentException("WhatsApp inválido.");

			// Email: validação básica usando MailAddress
			if (string.IsNullOrWhiteSpace(email))
				throw new ArgumentException("Email vazio.");
			email = email.Trim().ToLowerInvariant();
			try
			{
				_ = new MailAddress(email);
			}
			catch
			{
				throw new ArgumentException("Email inválido.");
			}

			// Instagram / Facebook: opcionais, apenas trim e comprimento máximo
			if (!string.IsNullOrWhiteSpace(instagram))
			{
				instagram = instagram.Trim();
				if (instagram.Length > 100) throw new ArgumentException("Instagram muito longo.");
			}
			if (!string.IsNullOrWhiteSpace(facebook))
			{
				facebook = facebook.Trim();
				if (facebook.Length > 200) throw new ArgumentException("Facebook muito longo.");
			}

			// Descrição do perfil: opcional, limite
			if (!string.IsNullOrWhiteSpace(descricaoPerfil))
			{
				descricaoPerfil = descricaoPerfil.Trim();
				if (descricaoPerfil.Length > 2000) throw new ArgumentException("DescricaoPerfil muito longa.");
			}

			// FotoUrl: opcional, se informado deve ser URL válida
			if (!string.IsNullOrWhiteSpace(fotoUrl))
			{
				fotoUrl = fotoUrl.Trim();
				if (!Uri.TryCreate(fotoUrl, UriKind.Absolute, out var u) || (u.Scheme != Uri.UriSchemeHttp && u.Scheme != Uri.UriSchemeHttps))
					throw new ArgumentException("FotoUrl inválida.");
			}
			else
			{
				fotoUrl = null!;
			}

			// Nicho de atuação: obrigatório
			if (string.IsNullOrWhiteSpace(nichoAtuacao))
				throw new ArgumentException("NichoAtuacao vazio.");
			nichoAtuacao = nichoAtuacao.Trim();

			// Endereço: CEP
			if (string.IsNullOrWhiteSpace(cep)) throw new ArgumentException("CEP vazio.");
			var cepDigits = new string(cep.Where(char.IsDigit).ToArray());
			if (cepDigits.Length != 8) throw new ArgumentException("CEP inválido.");
			cep = cepDigits;

			// Estado: dois caracteres (sigla)
			if (string.IsNullOrWhiteSpace(estado)) throw new ArgumentException("Estado vazio.");
			estado = estado.Trim().ToUpperInvariant();
			if (estado.Length != 2) throw new ArgumentException("Estado inválido. Use sigla de 2 caracteres.");

			// Cidade, Rua, Bairro obrigatórios
			if (string.IsNullOrWhiteSpace(cidade)) throw new ArgumentException("Cidade vazia.");
			cidade = cidade.Trim();
			if (string.IsNullOrWhiteSpace(rua)) throw new ArgumentException("Rua vazia.");
			rua = rua.Trim();
			if (string.IsNullOrWhiteSpace(bairro)) throw new ArgumentException("Bairro vazio.");
			bairro = bairro.Trim();

			// Numero e SemNumero
			if (!semNumero)
			{
				if (string.IsNullOrWhiteSpace(numero)) throw new ArgumentException("Numero vazio.");
				numero = numero.Trim();
			}
			else
			{
				numero = string.Empty;
			}

			// Complemento opcional
			if (!string.IsNullOrWhiteSpace(complemento))
				complemento = complemento.Trim();
			else
				complemento = string.Empty;

			// Monta entidade com valores normalizados
			return new Artesao
			{
				Id = Guid.NewGuid().ToString(),
				UsuarioId = usuarioId.Trim(),
				NomeCompleto = nomeCompleto,
				Idade = idade,
				NomeArtesao = nomeArtesao,
				Telefone = telefoneDigits,
				WhatsApp = whatsappDigits,
				Email = email,
				Instagram = instagram,
				Facebook = facebook,
				DescricaoPerfil = descricaoPerfil,
				ReceberEncomendas = receberEncomendas,
				EnviaEncomendas = enviaEncomendas,
				FotoUrl = fotoUrl,
				NichoAtuacao = nichoAtuacao,
				LocalFisico = localFisico,
				FeiraMunicipal = feiraMunicipal,
				CEP = cep,
				Estado = estado,
				Cidade = cidade,
				Rua = rua,
				Bairro = bairro,
				Complemento = complemento,
				Numero = numero,
				SemNumero = semNumero,
				ArtesanatoIds = new List<string>()
			};

		}


		#endregion


	}
}
