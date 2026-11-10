using FluentValidation;
using PrototipoBackEnd.Application.Dtos.Artesao;

namespace PrototipoBackEnd.Application.Validators
{
    public class ArtesaoDtoValidator : AbstractValidator<ArtesaoDto>
    {
        public ArtesaoDtoValidator()
        {
            RuleFor(x => x.PessoaId).NotEmpty().WithMessage("PessoaId é obrigatório.");
            RuleFor(x => x.Nome).NotEmpty().WithMessage("Nome é obrigatório.");
            RuleFor(x => x.Especialidade).NotNull().WithMessage("Especialidade é obrigatória.");
            RuleFor(x => x.Especialidade.Itens).Must(i => i != null && i.Count <= 10)
                .When(x => x.Especialidade != null).WithMessage("Máximo de 10 especialidades permitido.");
        }
    }
}
