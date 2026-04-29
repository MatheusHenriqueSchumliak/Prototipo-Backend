using PrototipoBackEnd.Domain.Entities.Base;
using PrototipoBackEnd.Domain.ValueObjects;

namespace PrototipoBackEnd.Domain.Entities
{
	public class Artesao : EntityBase
	{
		public string PessoaId { get; private set; } = string.Empty;
		public string Nome { get; private set; } = string.Empty;
		public string Descricao { get; private set; } = string.Empty;
		public string Foto { get; private set; } = string.Empty;
		public bool RecebeEncomenda { get; private set; }
		public bool EnviaEncomenda { get; private set; }
		public bool LocalFisico { get; private set; }
		public bool FeiraMunicipal { get; private set; }
		public Especialidade Especialidade { get; private set; } = null!;
		public Endereco? Endereco { get; private set; }
		public RedesSociais RedesSociais { get; private set; } = null!;

		public Artesao() { }

		public static Artesao Criar(string pessoaId, string nome, string descricao, string foto, Especialidade especialidade, Endereco? endereco, RedesSociais redesSociais, bool recebeEncomenda, bool enviaEncomenda, bool localFisico, bool feiraMunicipal)
		{
			if (string.IsNullOrWhiteSpace(pessoaId))
				throw new ArgumentException("PessoaId vazio.");

			if (string.IsNullOrWhiteSpace(nome))
				throw new ArgumentException("NomeArtesao vazio.");

			// REGRA local físico e endereço são dependentes um do outro
			if (localFisico && endereco == null)
				throw new ArgumentException("Endereço é obrigatório quando há local físico.");

			if (!localFisico && endereco != null)
				throw new ArgumentException("Endereço não deve ser informado sem local físico.");

			return new Artesao
			{
				Id = Guid.NewGuid().ToString(),
				PessoaId = pessoaId,
				Nome = nome.Trim(),
				Descricao = descricao.Trim(),
				Foto = foto.Trim(),
				Especialidade = especialidade,
				Endereco = endereco,
				RedesSociais = redesSociais,
				RecebeEncomenda = recebeEncomenda,
				EnviaEncomenda = enviaEncomenda,
				LocalFisico = localFisico,
				FeiraMunicipal = feiraMunicipal

			};
		}

		public void Atualizar(string? nome, string? descricao, string? foto, Endereco? endereco, RedesSociais? redesSociais, bool? recebeEncomenda, bool? enviaEncomenda, bool? localFisico, bool? feiraMunicipal)
		{
			if (!string.IsNullOrWhiteSpace(nome))
				Nome = nome.Trim();

			if (!string.IsNullOrWhiteSpace(descricao))
				Descricao = descricao.Trim();

			if (!string.IsNullOrWhiteSpace(foto))
				Foto = foto.Trim();

			if (redesSociais != null)
				RedesSociais = redesSociais;

			if (recebeEncomenda.HasValue)
				RecebeEncomenda = recebeEncomenda.Value;

			if (enviaEncomenda.HasValue)
				EnviaEncomenda = enviaEncomenda.Value;

			if (feiraMunicipal.HasValue)
				FeiraMunicipal = feiraMunicipal.Value;

			if (localFisico.HasValue)
				LocalFisico = localFisico.Value;

			//REGRA DO ENDEREÇO
			if (LocalFisico)
			{
				if (endereco == null)
					throw new ArgumentException("Endereço obrigatório quando há local físico.");

				Endereco = endereco;
			}
			else
			{
				Endereco = null;
			}

			MarcarAtualizado();
		}

		public void AtualizarEspecialidades(Especialidade especialidade)
		{
			Especialidade = especialidade;
			MarcarAtualizado();
		}

		public void Inativar()
		{
			if (DataRemocao.HasValue)
				return;

			RecebeEncomenda = false;
			EnviaEncomenda = false;
			LocalFisico = false;
			FeiraMunicipal = false;

			Endereco = null;

			Remover();
		}

		public void Reativar()
		{
			DataRemocao = null;
			MarcarAtualizado();
		}

	}
}