namespace PrototipoBackEnd.Application.Interfaces
{
    public interface IAdminSeedService
    {
        /// <summary>Cria o primeiro Administrador (e a Pessoa vinculada) se ainda não existir. Idempotente.</summary>
        /// <returns>true se criou; false se ignorou (sem configuração ou já existente).</returns>
        Task<bool> ExecutarAsync();
    }
}
