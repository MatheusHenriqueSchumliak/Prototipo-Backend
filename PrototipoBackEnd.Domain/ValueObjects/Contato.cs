using System.Net.Mail;

namespace PrototipoBackEnd.Domain.ValueObjects
{
	public class Contato
	{
		public string Telefone { get; }
		public string WhatsApp { get; }
		public string Email { get; }

		private Contato()
		{
			Telefone = string.Empty;
			WhatsApp = string.Empty;
			Email = string.Empty;
		}

		public Contato(string telefone, string whatsapp, string email)
		{
			static string NormalizePhone(string p)
			{
				if (string.IsNullOrWhiteSpace(p))
					return string.Empty;

				return string.Concat(p.Where(char.IsDigit));
			}

			var telefoneDigits = NormalizePhone(telefone);
			var whatsappDigits = NormalizePhone(whatsapp);

			if (string.IsNullOrWhiteSpace(telefoneDigits) || telefoneDigits.Length < 8)
				throw new ArgumentException("Telefone inválido.");

			if (string.IsNullOrWhiteSpace(whatsappDigits) || whatsappDigits.Length < 8)
				throw new ArgumentException("WhatsApp inválido.");

			if (string.IsNullOrWhiteSpace(email))
				throw new ArgumentException("Email vazio.");

			email = email.Trim().ToLower();

			try
			{
				_ = new MailAddress(email);
			}
			catch
			{
				throw new ArgumentException("Email inválido.");
			}

			Telefone = telefoneDigits;
			WhatsApp = whatsappDigits;
			Email = email;
		}

	}
}
