using PrototipoBackEnd.Application.Exceptions;

namespace PrototipoBackEnd.Application.Common.Helpers
{
    public static class IdValidator
    {
        // Os ids do sistema são GUIDs em texto; qualquer outro formato é erro do cliente (400), não "não encontrado".
        public static void Validar(string? id, string mensagem = "ID inválido.")
        {
            if (!Guid.TryParse(id, out _))
                throw new ValidationException(mensagem);
        }
    }
}
