namespace PrototipoBackEnd.Domain.Entities.Base
{
	public abstract class EntityBase
	{
		public string Id { get; protected set; }
		public DateTime DataCriacao { get; protected set; }
		public DateTime? DataAtualizacao { get; protected set; }
		public DateTime? DataRemocao { get; protected set; }
		public bool IsAtivo { get; protected set; }

		public EntityBase() { }

		public void MarcarAtualizado()
		{
			DataAtualizacao = DateTime.UtcNow;
		}

		public void Remover()
		{
			DataRemocao = DateTime.UtcNow;
			IsAtivo = false;
		}

	}
}
