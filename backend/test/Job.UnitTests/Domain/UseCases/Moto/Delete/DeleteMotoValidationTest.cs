using Job.Domain.UseCases.Moto.Delete.Commands;
using Job.Domain.UseCases.Moto.Delete.Commands.Validations;

namespace Job.UnitTests.Domain.UseCases.Moto.Delete;

[Trait("Moto", "Delete Validation")]
public sealed class DeleteMotoValidationTest
{
    [Fact]
    public void ShouldReturnErrorWhenCommandIsEmpty()
    {
        // Arrange
        var command = new DeleteMotoCommand(Guid.Empty);

        // Act
        var result = new DeleteMotoValidation().TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldNotReturnErrorWhenCommandIsValid()
    {
        // Arrange
        var command = new DeleteMotoCommand(Guid.NewGuid());

        // Act
        var result = new DeleteMotoValidation().TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}