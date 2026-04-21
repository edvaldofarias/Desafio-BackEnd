using FluentValidation.TestHelper;
using Job.Application.Commands.Rental.Validations;
using Job.Commons.Domain.Commands.Rent;

namespace Job.UnitTests.Application.Commands.Rental;

[Trait("Validation", "CancelRentalValidation")]
public class CancelRentalValidationTest
{
    private readonly CancelRentalValidation _validator = new();

    [Fact]
    public void ShouldReturnErrorWhenEmpty()
    {
        var command = CancelRentCommandFaker.Empty().Generate();
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Identifier);
        result.ShouldHaveValidationErrorFor(x => x.DateReturn);
    }

    [Fact]
    public void ShouldNotReturnErrorWhenValid()
    {
        var command = CancelRentCommandFaker.Default().Generate();
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Identifier);
        result.ShouldNotHaveValidationErrorFor(x => x.DateReturn);
    }
}
