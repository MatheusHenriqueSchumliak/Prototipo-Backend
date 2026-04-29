using PrototipoBackEnd.Domain.Entities.Base;
using PrototipoBackEnd.Domain.Enumerables;

namespace PrototipoBackEnd.Domain.Entities
{
	public class Usuario : EntityBase
	{
		public string PessoaId { get; private set; } = string.Empty;
		public string Email { get; private set; } = string.Empty;
		public string SenhaHash { get; private set; } = string.Empty;
		public UsuarioEnum Role { get; private set; }

		public Usuario() { }

		public static Usuario Criar(string pessoaId, string email, string senhaHash, UsuarioEnum role)
		{
			if (string.IsNullOrWhiteSpace(pessoaId))
				throw new ArgumentException("PessoaId vazio.");

			if (string.IsNullOrWhiteSpace(email))
				throw new ArgumentException("Email vazio.");

			if (string.IsNullOrWhiteSpace(senhaHash))
				throw new ArgumentException("Senha inválida.");

			email = email.Trim().ToLower();

			//validação simples de email
			if (!email.Contains('@'))
				throw new ArgumentException("Email inválido.");

			return new Usuario
			{
				Id = Guid.NewGuid().ToString(),
				PessoaId = pessoaId,
				Email = email,
				SenhaHash = senhaHash,
				Role = role,
				DataCriacao = DateTime.UtcNow
			};
		}

		public void AlterarEmail(string email)
		{
			if (string.IsNullOrWhiteSpace(email))
				throw new ArgumentException("Email vazio.");

			email = email.Trim().ToLower();

			if (!email.Contains('@'))
				throw new ArgumentException("Email inválido.");

			Email = email;
			MarcarAtualizado();
		}

		public void AlterarSenha(string novoHash)
		{
			if (string.IsNullOrWhiteSpace(novoHash))
				throw new ArgumentException("Senha inválida.");

			SenhaHash = novoHash;
			MarcarAtualizado();
		}

		public void AlterarRole(UsuarioEnum role)
		{
			Role = role;
			MarcarAtualizado();
		}

		public void Inativar()
		{
			if (DataRemocao.HasValue)
				return;

			Remover();
		}

		public void Reativar()
		{
			DataRemocao = null;
			MarcarAtualizado();
		}

	}
}
