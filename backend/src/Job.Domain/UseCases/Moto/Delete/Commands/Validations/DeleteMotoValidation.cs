namespace Job.Domain.UseCases.Moto.Delete.Commands.Validations;

public sealed class DeleteMotoValidation : AbstractValidator<DeleteMotoCommand>
{
    public DeleteMotoValidation()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id é obrigatório.");
    }
}