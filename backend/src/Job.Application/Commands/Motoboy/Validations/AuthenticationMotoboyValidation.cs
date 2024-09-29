﻿namespace Job.Application.Commands.Motoboy.Validations;

public sealed class AuthenticationMotoboyValidation : AbstractValidator<AuthenticationMotoboyCommand>
{
    public AuthenticationMotoboyValidation()
    {
        RuleFor(x => x.Cnpj)
            .NotEmpty()
            .WithMessage("Cnpj é obrigatório")
            .Must(CnpjValidation.IsCnpj)
            .WithMessage("Cnpj inválido");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Senha é obrigatório")
            .MinimumLength(6)
            .WithMessage("Senha deve ter no mínimo 6 caracteres");
    }
}