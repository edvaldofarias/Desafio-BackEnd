namespace Job.Application.Commands.Manager.Validations;

public sealed class AuthenticationManagerValidation : AbstractValidator<AuthenticationManagerCommand>
{
    public AuthenticationManagerValidation()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email é obrigatório")
            .EmailAddress()
            .WithMessage("Email é invalido");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Senha é obrigatório")
            .MinimumLength(6)
            .WithMessage("Senha deve ter no mínimo 6 caracteres");
    }
}