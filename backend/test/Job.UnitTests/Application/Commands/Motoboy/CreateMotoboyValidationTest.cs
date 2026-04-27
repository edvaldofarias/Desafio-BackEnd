using FluentValidation.TestHelper;
using Job.Application.Commands.Motoboy.Validations;
using Job.Commons.Domain.Commands.User.Motoboy;

namespace Job.UnitTests.Application.Commands.Motoboy;

[Trait("Validation", "CreateMotoboyValidation")]
public class CreateMotoboyValidationTest
{
    private readonly CreateMotoboyValidation _validator = new();

    [Fact]
    public void ShouldReturnErrorWhenInvalid()
    {
        var command = CreateMotoboyCommandFaker.Invalid().Generate();
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Cnpj);
        result.ShouldHaveValidationErrorFor(x => x.Password);
        result.ShouldHaveValidationErrorFor(x => x.DateBirth);
        result.ShouldHaveValidationErrorFor(x => x.Cnh);
        result.ShouldHaveValidationErrorFor(x => x.TypeCnh);
    }

    [Fact]
    public void ShouldNotReturnErrorWhenValid()
    {
        var command = CreateMotoboyCommandFaker.Default().Generate();
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Cnpj);
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
        result.ShouldNotHaveValidationErrorFor(x => x.DateBirth);
        result.ShouldNotHaveValidationErrorFor(x => x.Cnh);
        result.ShouldNotHaveValidationErrorFor(x => x.TypeCnh);
    }
}
