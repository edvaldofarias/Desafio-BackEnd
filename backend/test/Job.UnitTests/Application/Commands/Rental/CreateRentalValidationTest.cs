using FluentValidation.TestHelper;
using Job.Application.Commands.Rental.Validations;
using Job.Commons.Domain.Commands.Rent;

namespace Job.UnitTests.Application.Commands.Rental;

[Trait("Validation", "CreateRentalValidation")]
public class CreateRentalValidationTest
{
    private readonly CreateRentalValidation _validator = new();

    [Fact]
    public void ShouldReturnErrorWhenEmpty()
    {
        var command = CreateRentCommandFaker.Empty().Generate();
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.MotoIdentifier);
        result.ShouldHaveValidationErrorFor(x => x.Plan);
    }

    [Fact]
    public void ShouldNotReturnErrorWhenValid()
    {
        var command = CreateRentCommandFaker.Default().Generate();
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.MotoIdentifier);
        result.ShouldNotHaveValidationErrorFor(x => x.Plan);
    }
}
