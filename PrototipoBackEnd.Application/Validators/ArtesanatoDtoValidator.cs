using FluentValidation;
using PrototipoBackEnd.Application.Dtos.Artesanato;

namespace PrototipoBackEnd.Application.Validators
{
    public class ArtesanatoDtoValidator : AbstractValidator<ArtesanatoDto>
    {
        public ArtesanatoDtoValidator()
        {
            RuleFor(x => x.ArtesaoId).NotEmpty().WithMessage("ArtesaoId é obrigatório.");
            RuleFor(x => x.Titulo).NotEmpty().WithMessage("Título é obrigatório.");
            RuleFor(x => x.Preco).GreaterThanOrEqualTo(0).When(x => x.Preco.HasValue).WithMessage("Preço não pode ser negativo.");
            RuleFor(x => x.Quantidade).GreaterThanOrEqualTo(0).WithMessage("Quantidade inválida.");
            RuleFor(x => x.TempoCriacao).GreaterThanOrEqualTo(0).WithMessage("Tempo de criação inválido.");
        }
    }
}
