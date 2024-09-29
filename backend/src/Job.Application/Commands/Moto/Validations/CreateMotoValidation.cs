﻿namespace Job.Application.Commands.Moto.Validations;

public sealed class CreateMotoValidation : AbstractValidator<CreateMotoCommand>
{
    public CreateMotoValidation()
    {
        RuleFor(x => x.Year)
            .NotEmpty()
            .WithMessage("Ano é obrigatório")
            .GreaterThan(1900)
            .WithMessage("Ano deve ser maior que 1900");

        RuleFor(x => x.Model)
            .NotEmpty()
            .WithMessage("Modelo é obrigatório")
            .MinimumLength(3)
            .WithMessage("Modelo deve ter no mínimo 3 caracteres");

        RuleFor(x => x.Plate)
            .NotEmpty()
            .WithMessage("Placa é obrigatória")
            .Matches("[a-zA-Z]{3}[0-9]{1}[a-zA-Z]{1}[0-9]{2}|[a-zA-Z]{3}[0-9]{4}")
            .WithMessage("Placa inválida");
    }
}