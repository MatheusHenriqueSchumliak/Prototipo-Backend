using PrototipoBackEnd.Domain.Interfaces;

namespace PrototipoBackEnd.Tests.Integration.Repositories
{
    public class UnitOfWorkFake : IUnitOfWork
    {
        public Task ExecutarAsync(Func<Task> operacao) => operacao();
    }
}
