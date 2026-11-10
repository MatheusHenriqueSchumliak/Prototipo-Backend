using FluentValidation;
using PrototipoBackEnd.Application.Dtos.Usuario;

namespace PrototipoBackEnd.Application.Validators
{
    public class UsuarioRequestDtoValidator : AbstractValidator<UsuarioRequestDto>
    {
        public UsuarioRequestDtoValidator()
        {
            RuleFor(x => x.PessoaId).NotEmpty().WithMessage("PessoaId é obrigatório.");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("E-mail inválido.");
            RuleFor(x => x.Senha).NotEmpty().Length(8, 72).WithMessage("A senha deve ter entre 8 e 72 caracteres.");
            RuleFor(x => x.Role).IsInEnum().WithMessage("Role inválida.");
        }
    }

    public class UsuarioAtualizarRequestDtoValidator : AbstractValidator<UsuarioAtualizarRequestDto>
    {
        public UsuarioAtualizarRequestDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("E-mail inválido.");
            RuleFor(x => x.Role).IsInEnum().WithMessage("Role inválida.");
            RuleFor(x => x.Senha).Length(8, 72).When(x => !string.IsNullOrWhiteSpace(x.Senha))
                .WithMessage("A senha deve ter entre 8 e 72 caracteres.");
        }
    }
}
