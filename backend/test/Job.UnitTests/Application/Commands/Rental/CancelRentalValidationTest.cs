﻿using Job.Application.Commands.Rental.Validations;
using Job.Commons.Domain.Commands.Rent;

namespace Job.UnitTests.Application.Commands.Rental;

[Trait("Validation", "CancelRentalValidation")]
public class CancelRentalValidationTest
{
    private readonly CancelRentalValidation _validator = new();

    [Fact]
    public void ShouldReturnErrorWhenIdRentIsEmpty()
    {
        // Arrange
        var command = CancelRentCommandFaker.Empty().Generate();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
        result.ShouldHaveValidationErrorFor(x => x.DatePreview);
    }

    [Fact]
    public void ShouldNotReturnErrorWhenCommandIsValid()
    {
        // Arrange
        var command = CancelRentCommandFaker.Default().Generate();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
        result.ShouldNotHaveValidationErrorFor(x => x.DatePreview);
    }


}