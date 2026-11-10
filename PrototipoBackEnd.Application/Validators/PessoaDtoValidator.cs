using FluentValidation;
using PrototipoBackEnd.Application.Dtos.Pessoa;

namespace PrototipoBackEnd.Application.Validators
{
    public class PessoaDtoValidator : AbstractValidator<PessoaDto>
    {
        public PessoaDtoValidator()
        {
            RuleFor(x => x.NomeCompleto).NotEmpty().WithMessage("Nome completo é obrigatório.")
                .MaximumLength(150).WithMessage("Nome completo muito longo.");
            RuleFor(x => x.DataNascimento).LessThan(DateTime.UtcNow).WithMessage("Data de nascimento inválida.");
            RuleFor(x => x.Endereco).NotNull().WithMessage("Endereço é obrigatório.");
            RuleFor(x => x.Contato).NotNull().WithMessage("Contato é obrigatório.");
            RuleFor(x => x.Contato.Email).EmailAddress().When(x => x.Contato != null && !string.IsNullOrWhiteSpace(x.Contato.Email))
                .WithMessage("E-mail de contato inválido.");
        }
    }
}
