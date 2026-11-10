namespace PrototipoBackEnd.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        // Executa a operação de forma atômica: tudo é confirmado ou nada é persistido.
        Task ExecutarAsync(Func<Task> operacao);
    }
}
