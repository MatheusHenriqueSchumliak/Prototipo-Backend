using PrototipoBackEnd.Domain.Entities;
using PrototipoBackEnd.Domain.ValueObjects;

namespace PrototipoBackEnd.Tests
{
    public static class PessoaTestes
    {
        public static Pessoa Criar(string? id = null)
        {
            var pessoa = Pessoa.Criar(
                "Fulano de Tal",
                new DateTime(1990, 1, 1),
                new Endereco("12345678", "SP", "São Paulo", "Rua Exemplo", "Centro", "100", null, false),
                new Contato("11999999999", "11999999999", "fulano@email.com"));

            if (id != null)
                typeof(Pessoa).GetProperty("Id")!.SetValue(pessoa, id);

            return pessoa;
        }
    }
}
