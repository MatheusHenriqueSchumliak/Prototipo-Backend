using PrototipoBackEnd.Domain.Entities.Base;
using PrototipoBackEnd.Domain.ValueObjects;

namespace PrototipoBackEnd.Domain.Entities
{
	public class Pessoa : EntityBase
	{
		public string NomeCompleto { get; private set; } = string.Empty;
		public DateTime DataNascimento { get; private set; }
		public Endereco Endereco { get; private set; } = null!;
		public Contato Contato { get; private set; } = null!;
		public bool TemUsuario { get; private set; }
		public bool EhArtesao { get; private set; }
		public Pessoa() { }

		public static Pessoa Criar(string nomeCompleto, DateTime dataNascimento, Endereco endereco, Contato contato)
		{
			if (string.IsNullOrWhiteSpace(nomeCompleto))
				throw new ArgumentException("NomeCompleto vazio.");

			if (endereco == null)
				throw new ArgumentException("Endereço obrigatório.");

			if (contato == null)
				throw new ArgumentException("Contato obrigatório.");

			// REGRA idade mínima
			var idade = DateTime.Today.Year - dataNascimento.Year;
			if (dataNascimento > DateTime.Today.AddYears(-idade))
				idade--;

			if (idade < 18)
				throw new ArgumentException("Pessoa deve ter pelo menos 18 anos.");

			return new Pessoa
			{
				Id = Guid.NewGuid().ToString(),
				NomeCompleto = nomeCompleto.Trim(),
				DataNascimento = dataNascimento,
				Contato = contato,
				Endereco = endereco,
				TemUsuario = false,
				EhArtesao = false,
				DataCriacao = DateTime.UtcNow
			};
		}

		public void Atualizar(string? nomeCompleto, DateTime? dataNascimento, Endereco? endereco, Contato? contato)
		{
			if (!string.IsNullOrWhiteSpace(nomeCompleto))
				NomeCompleto = nomeCompleto.Trim();

			if (dataNascimento.HasValue)
			{
				var idade = DateTime.Today.Year - dataNascimento.Value.Year;
				if (dataNascimento.Value > DateTime.Today.AddYears(-idade))
					idade--;

				if (idade < 18)
					throw new ArgumentException("Pessoa deve ter pelo menos 18 anos.");

				DataNascimento = dataNascimento.Value;
			}

			if (endereco != null)
				Endereco = endereco;

			if (contato != null)
				Contato = contato;

			MarcarAtualizado();
		}

		public void VincularUsuario()
		{
			if (TemUsuario)
				throw new InvalidOperationException("Pessoa já possui usuário.");

			TemUsuario = true;
			MarcarAtualizado();
		}

		public void TornarArtesao()
		{
			if (EhArtesao)
				throw new InvalidOperationException("Pessoa já é artesão.");

			EhArtesao = true;
			MarcarAtualizado();
		}

		public void RemoverArtesao()
		{
			if (!EhArtesao)
				return;

			EhArtesao = false;
			MarcarAtualizado();
		}

		public void Inativar()
		{
			if (DataRemocao.HasValue)
				return;

			TemUsuario = false;
			EhArtesao = false;

			Remover();
		}

		public void Reativar()
		{
			DataRemocao = null;
			MarcarAtualizado();
		}

	}
}
