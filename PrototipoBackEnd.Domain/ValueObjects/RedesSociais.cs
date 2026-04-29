namespace PrototipoBackEnd.Domain.ValueObjects
{
	public class RedesSociais
	{
		public string? Instagram { get; }
		public string? Facebook { get; }

		private RedesSociais() { }
		public RedesSociais(string? instagram, string? facebook)
		{
			if (!string.IsNullOrWhiteSpace(instagram))
			{
				instagram = instagram.Trim();
				if (instagram.Length > 100)
					throw new ArgumentException("Instagram muito longo.");
			}
			else
			{
				instagram = null;
			}

			if (!string.IsNullOrWhiteSpace(facebook))
			{
				facebook = facebook.Trim();
				if (facebook.Length > 200)
					throw new ArgumentException("Facebook muito longo.");
			}
			else
			{
				facebook = null;
			}

			Instagram = instagram;
			Facebook = facebook;
		}

	}
}
