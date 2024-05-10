using Job.Commons.Domain.Commands.Moto;
using Job.Commons.Domain.Entities.Moto;
using Job.Domain.Entities.Moto;
using Job.Domain.UseCases.Moto.Update.Commands;
using Job.Domain.UseCases.Moto.Update.Commands.Validations;
using Job.Domain.UseCases.Moto.Update.Services;

namespace Job.UnitTests.Domain.UseCases.Moto.Update;

[Trait("MotoService", "Update")]
public class MotoServiceTest
{
    private readonly Mock<IMotoRepository> _motoRepository = new();
    private readonly Mock<ILogger<MotoService>> _logger = new();
    private readonly IValidator<UpdateMotoCommand> _validator = new UpdateMotoValidation();
    private readonly MotoService _motoService;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public MotoServiceTest()
    {
        _motoService = new MotoService(_logger.Object, _validator, _motoRepository.Object);
    }

    [Fact]
    public async Task UpdateAsync_WhenCommandIsValid_ShouldUpdateMoto()
    {
        // Arrange
        var entity = MotoEntityFaker.Default().Generate();
        var command = UpdateMotoCommandFaker.Default().Generate();
        _motoRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), _cancellationToken))
            .ReturnsAsync(entity);

        // Act
        var response = await _motoService.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        _motoRepository.Verify(x => x.UpdateAsync(It.IsAny<MotoEntity>(), _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenMotoNotFound_ShouldReturnError()
    {
        // Arrange
        var command = UpdateMotoCommandFaker.Default().Generate();
        _motoRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), _cancellationToken))
            .ReturnsAsync((MotoEntity?)null);

        // Act
        var response = await _motoService.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.Success);
        _motoRepository.Verify(x => x.UpdateAsync(It.IsAny<MotoEntity>(), _cancellationToken), Times.Never);
    }
}