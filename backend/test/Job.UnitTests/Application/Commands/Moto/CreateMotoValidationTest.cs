using FluentValidation.TestHelper;
using Job.Application.Commands.Moto.Validations;
using Job.Commons.Application.Commands.Moto;

namespace Job.UnitTests.Application.Commands.Moto;

[Trait("Validation", "CreateMotoValidation")]
public class CreateMotoValidationTest
{
    private readonly CreateMotoValidation _validator = new();

    [Fact]
    public void ShouldReturnErrorWhenCommandIsEmpty()
    {
        var command = CreateMotoCommandFaker.Empty().Generate();
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Identifier);
        result.ShouldHaveValidationErrorFor(x => x.Year);
        result.ShouldHaveValidationErrorFor(x => x.Model);
        result.ShouldHaveValidationErrorFor(x => x.Plate);
    }

    [Fact]
    public void ShouldNotReturnErrorWhenCommandIsValid()
    {
        var command = CreateMotoCommandFaker.Default().Generate();
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Identifier);
        result.ShouldNotHaveValidationErrorFor(x => x.Year);
        result.ShouldNotHaveValidationErrorFor(x => x.Model);
        result.ShouldNotHaveValidationErrorFor(x => x.Plate);
    }
}
