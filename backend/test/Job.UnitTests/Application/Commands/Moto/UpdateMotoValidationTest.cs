using Job.Application.Commands.Moto.Validations;
using Job.Commons.Application.Commands.Moto;

namespace Job.UnitTests.Application.Commands.Moto;

[Trait("Validation", "UpdateMotoValidation")]
public class UpdateMotoValidationTest
{
    private readonly UpdateMotoValidation _validator = new();

    [Fact]
    public void ShouldReturnErrorWhenCommandIsEmpty()
    {
        var command = UpdateMotoCommandFaker.Empty().Generate();

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
        result.ShouldHaveValidationErrorFor(x => x.Plate);
    }

    [Fact]
    public void ShouldReturnErrorWhenPlateIsInvalid()
    {
        var command = UpdateMotoCommandFaker.Invalid().Generate();

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
        result.ShouldHaveValidationErrorFor(x => x.Plate);
    }

    [Fact]
    public void ShouldNotReturnErrorWhenCommandIsValid()
    {
        var command = UpdateMotoCommandFaker.Default().Generate();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Id);
        result.ShouldNotHaveValidationErrorFor(x => x.Plate);
    }
}