﻿using Job.Application.Commands.Motoboy.Validations;
using Job.Commons.Domain.Commands.User.Motoboy;

namespace Job.UnitTests.Application.Commands.Motoboy;

[Trait("Validation", "AuthenticationMotoboyValidation")]
public class AuthenticationMotoboyValidationTest
{
    private readonly AuthenticationMotoboyValidation _validator = new();

    [Fact]
    public void ShouldReturnErrorWhenCnpjIsInvalid()
    {
        // Arrange
        var command = AuthenticationMotoboyCommandFaker.Invalid().Generate();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Cnpj);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void ShouldNotReturnErrorWhenCommandIsValid()
    {
        // Arrange
        var command = AuthenticationMotoboyCommandFaker.Default().Generate();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Cnpj);
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }


}